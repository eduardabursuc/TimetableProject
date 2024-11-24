using Application.Validators;
using Domain.Entities;
using System.Data;

namespace Application.Services
{
    public class ArcConsistency(Instance instance)
    {
        private readonly ConstraintsValidator _constraintsValidator = new(instance);

        public bool ApplyArcConsistencyAndBacktracking(out Dictionary<Event, (Room, Timeslot)> solution)
        {
            GenerateVariablesAndDomains(out var variables);

            if (Ac3(variables))
            {
                solution = new Dictionary<Event, (Room, Timeslot)>();
                return Backtrack(variables, solution);
            }
            else
            {
                solution = null!;
                return false;
            }
        }

        private void GenerateVariablesAndDomains(out Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            variables = new Dictionary<Event, List<(Room, Timeslot)>>();

            foreach (var ev in instance.Events)
            {
                var possibleRooms = instance.Rooms.Where(r => IsRoomCapacitySufficient(r, ev.EventName)).ToList();
                var possibleTimeslots = instance.TimeSlots;

                var possibleValues = (from room in possibleRooms from timeslot in possibleTimeslots select (room, timeslot)).ToList();

                variables[ev] = possibleValues;
            }
        }

        private bool Ac3(Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            var queue = new Queue<(Event, Event)>();
            foreach (var var1 in variables.Keys)
            {
                foreach (var var2 in variables.Keys.Where(var2 => var1 != var2))
                {
                    queue.Enqueue((var1, var2));
                }
            }

            while (queue.Count > 0)
            {
                var (var1, var2) = queue.Dequeue();
                if (!Revise(var1, var2, variables, true)) continue;
                if (variables[var1].Count == 0)
                {
                    return false;
                }

                foreach (var var3 in variables.Keys.Where(var3 => var3 != var1 && var3 != var2))
                {
                    queue.Enqueue((var3, var1));
                }
            }

            return true;
        }

        private bool Backtrack(Dictionary<Event, List<(Room, Timeslot)>> variables, Dictionary<Event, (Room, Timeslot)> assignment)
        {
            if (variables.Count == assignment.Count)
            {
                return true;
            }

            var unassigned = variables.Keys.FirstOrDefault(v => !assignment.ContainsKey(v));
            if (unassigned == null)
            {
                return false;
            }

            foreach (var value in variables[unassigned].Where(value => IsAssignmentConsistent(unassigned, value, assignment)))
            {
                assignment[unassigned] = value;

                if (Backtrack(variables, assignment))
                {
                    return true;
                }

                Console.WriteLine($"Backtracking on {unassigned}");
                assignment.Remove(unassigned);
            }

            return false;
        }

        private bool Revise(Event var1, Event var2, Dictionary<Event, List<(Room, Timeslot)>> variables, bool applySoftConstraints)
        {
            var revised = false;
            var domain1 = variables[var1];
            var domain2 = variables[var2];

            for (int i = domain1.Count - 1; i >= 0; i--)
            {
                var value1 = domain1[i];
                var consistent = domain2.Any(value2 => IsConsistent(var1, value1, var2, value2, applySoftConstraints));

                if (consistent) continue;
                domain1.RemoveAt(i);
                revised = true;
            }

            return revised;
        }

        private bool IsAssignmentConsistent(Event var1, (Room, Timeslot) value1, Dictionary<Event, (Room, Timeslot)> assignment)
        {
            return !(from var2 in assignment.Keys let value2 = assignment[var2] where !IsConsistent(var1, value1, var2, value2, applySoftConstraints: true) select var2).Any();
        }


        private bool IsConsistent(Event var1, (Room, Timeslot) value1, Event var2, (Room, Timeslot) value2, bool applySoftConstraints)
        {
            // Check if the same room and timeslot are assigned to different events
            if (value1.Item1 == value2.Item1 && value1.Item2 == value2.Item2)
            {
               return false;
            }

            // Check HARD_ROOM_CAPACITY constraint
            if (!IsRoomCapacitySufficient(value1.Item1, var1.EventName) || !IsRoomCapacitySufficient(value2.Item1, var2.EventName))
            {
               return false;
            }

            // Check if the events are from the same or nested group
            if (IsSameOrNestedGroup(var1.Group, var2.Group))
            {
                var course1 = instance.Courses.FirstOrDefault(c => c.CourseName == var1.CourseName);
                var course2 = instance.Courses.FirstOrDefault(c => c.CourseName == var2.CourseName);

                if (course1 != null && course2 != null)
                {
                    // Check if both courses are in the 'compulsory' package and have the same level and semester
                    if (course1.Package == "compulsory" && course2.Package == "compulsory" && course1.Level == course2.Level && course1.Semester == course2.Semester)
                    {
                        if (value1.Item1 == value2.Item1 && value1.Item2 == value2.Item2)
                        {
                            return false;
                        }
                    }
                    // Check if courses are in different packages but with the same level and semester
                    else if (course1.Package != course2.Package && course1.Level == course2.Level && course1.Semester == course2.Semester)
                    {
                        if (value1.Item1 == value2.Item1 && value1.Item2 == value2.Item2)
                        {
                            return false;
                        }
                    }
                    // Check if the events are the same course but different types (course vs. laboratory)
                    else if (var1.CourseName == var2.CourseName && var1.EventName != var2.EventName)
                    {
                        // Laboratories can overlap with each other but not with the course
                        if (var1.EventName == "course" || var2.EventName == "course")
                        {
                            if (value1.Item1 == value2.Item1 && value1.Item2 == value2.Item2)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // Events from the same or nested group cannot overlap
                        if (value1.Item1 == value2.Item1 && value1.Item2 == value2.Item2)
                        {
                               return false;
                        }
                    }
                }
            }

            // Check soft constraints if applicable
            if (!applySoftConstraints) return true;
            foreach (var result in instance.Constraints.Select(constraint => _constraintsValidator.Validate(constraint)).Where(result => !result.Item1))
            {
                // Log or handle the soft constraint violation if needed
                // But do not return false
            }

            return true;
        }




        private static bool IsSameOrNestedGroup(string group1, string group2)
        {
            // Check if one group is a prefix of the other (e.g., 2E and 2E3, 2MISS and 2MISS1)
            return group1.StartsWith(group2) || group2.StartsWith(group1);
        }

        private bool IsYearPriorityValid(string groupName1, string groupName2)
        {
            var year1 = GetYearFromGroupName(groupName1);
            var year2 = GetYearFromGroupName(groupName2);

            var isBachelor1 = IsBachelor(groupName1);
            var isBachelor2 = IsBachelor(groupName2);

            return isBachelor1 switch
            {
                true when !isBachelor2 => true,
                false when isBachelor2 => false,
                _ => year1 <= year2
            };

            // If both are Bachelor or both are Master, compare years
        }

        private static int GetYearFromGroupName(string groupName)
        {
            // Extract the year from the first character of the group name (e.g., "2B3" -> 2, "2MSI2" -> 2)
            return char.IsDigit(groupName[0]) ? int.Parse(groupName[0].ToString()) : int.MaxValue; // Default to a very high year if no digit is found
        }

        private static bool IsBachelor(string groupName)
        {
            // Determine if the group is Bachelor or Master based on the second character of the group name
            var level = groupName[1];
            return level != 'M';
        }

        private static bool IsRoomCapacitySufficient(Room room, string eventName)
        {
            return eventName.ToLower() switch
            {
                "course" => room.Capacity > 90,
                "seminary" => room.Capacity > 30,
                "laboratory" => room.Capacity > 30,
                _ => true
            };
        }

        public static void PrintSolution(Dictionary<Event, (Room, Timeslot)> solution)
        {
            foreach (var kvp in solution)
            {
                var ev = kvp.Key;
                var (room, timeslot) = kvp.Value;

                Console.WriteLine($"Event: {ev.EventName}, Course: {ev.CourseName}, Group: {ev.Group}");
                Console.WriteLine($"Assigned Room: {room.Name}");
                Console.WriteLine($"Assigned Timeslot: Day: {timeslot.Day}, Time: {timeslot.Time}");
                Console.WriteLine();
            }
        }
    }
}