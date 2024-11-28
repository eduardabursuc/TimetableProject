using Application.Validators;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.Services
{
    public class ArcConsistency
    {
        private readonly Instance _instance;
        private readonly ApplicationDbContext _dbContext;

        public ArcConsistency(Instance instance, ApplicationDbContext dbContext)
        {
            _instance = instance;
            _dbContext = dbContext;
        }
        
        public bool ApplyArcConsistencyAndBacktracking(out Dictionary<Event, (Room, Timeslot)> solution)
        {
            GenerateVariablesAndDomains(out var variables);

            if (Ac3(variables))
            {
                solution = new Dictionary<Event, (Room, Timeslot)>();
                if (Backtrack(variables, solution))
                {
                    SaveSolutionToDatabase(solution);
                    return true;
                }
            }

            solution = null!;
            return false;
        }
        
        private void SaveSolutionToDatabase(Dictionary<Event, (Room, Timeslot)> solution)
        {
            foreach (var kvp in solution)
            {
                var eventEntity = kvp.Key;
                var (room, timeslot) = kvp.Value;

                var timetableEntry = new Timetable
                {
                    Day = timeslot.Day,
                    Time = timeslot.Time,
                    CourseName = eventEntity.CourseName,
                    EventName = eventEntity.EventName,
                    Group = eventEntity.Group,
                    RoomName = room.Name,
                    Credits = _instance.Courses.First(c => c.CourseName == eventEntity.CourseName).Credits,
                    IsOptional = _instance.Courses.First(c => c.CourseName == eventEntity.CourseName).Level == "Optional"
                };

                _dbContext.Timetables.Add(timetableEntry);
            }

            _dbContext.SaveChanges();
        }

        private void GenerateVariablesAndDomains(out Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            variables = new Dictionary<Event, List<(Room, Timeslot)>>();

            foreach (var ev in _instance.Events)
            {
                var possibleRooms = _instance.Rooms;
                var possibleTimeslots = _instance.TimeSlots;

                var possibleValues = (from room in possibleRooms
                                      from timeslot in possibleTimeslots
                                      select (room, timeslot)).ToList();

                variables[ev] = possibleValues;
            }
            
            // HARD_YEAR_PRIORITY
            var sortedVariables = variables.OrderBy(kvp => GetPriority(kvp.Key.Group)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value); 
            variables = sortedVariables;
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
                if (!Revise(var1, var2, variables, applySoftConstraints: true)) continue;
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

                // Remove inconsistent value from domain1
                domain1.RemoveAt(i);
                revised = true;
            }

            return revised;
        }

        private bool IsAssignmentConsistent(Event var1, (Room, Timeslot) value1, Dictionary<Event, (Room, Timeslot)> assignment)
        {
            return (from var2 in assignment.Keys let value2 = assignment[var2] select IsConsistent(var1, value1, var2, value2, applySoftConstraints: true)).All(consistent => consistent);
        }


        private bool IsConsistent(Event var1, (Room, Timeslot) value1, Event var2, (Room, Timeslot) value2, bool applySoftConstraints)
        {
            // Check if the same room and timeslot are assigned to different events
            if (value1.Item1 == value2.Item1 && value1.Item2 == value2.Item2)
            {
                return false;
            }
            
            // HARD constraints validation
            HardConstraintValidator hardConstraintValidator = new(_instance);
            var valid = hardConstraintValidator.ValidateGroupOverlap(var1, var2, value1, value2)
                        && hardConstraintValidator.ValidateRoomCapacity(value1.Item1, var1.EventName)
                        && hardConstraintValidator.ValidateRoomCapacity(value2.Item1, var2.EventName);
            
            if (!valid) return false;

            // SOFT constraints validation
            if (!applySoftConstraints || _instance.Constraints.Count == 0) return true;
            
            SoftConstraintValidator softConstraintValidator = new(_instance);

            foreach (var constraint in _instance.Constraints)
            {
                valid = softConstraintValidator.ValidateLectureBeforeLabs(constraint, var1, var2, value1.Item2, value2.Item2)
                            && softConstraintValidator.ValidateConsecutiveHours(constraint, var1, var2, value1.Item2, value2.Item2)
                            && softConstraintValidator.Validate(constraint, var1, value1)
                            && softConstraintValidator.Validate(constraint, var2, value2);
                    
            }
            return valid;
        }
        

        public static void PrintSolution(Dictionary<Event, (Room, Timeslot)> solution)
        {
            SortSolution(solution, out var sortedSolution);
            foreach (var kvp in sortedSolution)
            {
                var ev = kvp.Key;
                var (room, timeslot) = kvp.Value;

                Console.WriteLine($"Event: {ev.EventName}, Course: {ev.CourseName}, Group: {ev.Group}, Professor: {ev.ProfessorId}");
                Console.WriteLine($"Assigned Room: {room.Name}");
                Console.WriteLine($"Assigned Timeslot: Day: {timeslot.Day}, Time: {timeslot.Time}");
                Console.WriteLine();
            }
        }
        

        private static void SortSolution(Dictionary<Event, (Room, Timeslot)> inputSolution, out Dictionary<Event, (Room, Timeslot)> sortedSolution)
        {
            // Initialize the out parameter
            sortedSolution = new Dictionary<Event, (Room, Timeslot)>();

            // Perform the sorting and assignment
            sortedSolution = inputSolution.OrderBy(kvp => kvp.Value.Item2.Day)
                .ThenBy(kvp => kvp.Value.Item2.Time)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        
        private int GetPriority(string groupName) 
        { 
            // Extract the year from the first character of the group name
            var year = char.IsDigit(groupName[0]) ? int.Parse(groupName[0].ToString()) : int.MaxValue; 
            // Check if the group is a master group
            var isMaster = groupName.Length > 1 && groupName[1] == 'M'; 
            // Assign higher priority to bachelor groups and lower to master groups
            return isMaster ? year + 10 : year; 
        }
        

    }
}
