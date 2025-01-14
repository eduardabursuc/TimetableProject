using Application.Validators;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Services
{
    public class TimetableGeneratorService(
        string userEmail,
        Instance instance,
        IRoomRepository roomRepository,
        IGroupRepository groupRepository,
        ICourseRepository courseRepository,
        IConstraintRepository constraintRepository,
        IProfessorRepository professorRepository,
        string timetableName)
    {
        private readonly List<Constraint> softConstraints = new List<Constraint>();
        private readonly SoftConstraintsValidator softConstraintsValidator = new SoftConstraintsValidator();
        private readonly Dictionary<int, List<Timeslot>> timeslotCache = new Dictionary<int, List<Timeslot>>();


        public async Task<Timetable> GenerateBestTimetableAsync()
        {
            var (variables, _) = await GenerateVariablesWithConstraintsAsync();
            Console.WriteLine("Trying generating an optimal solution...");

            var optimalSolution = await FindOptimalSolutionWithBacktrackingAsync(variables);

            if (optimalSolution != null)
            {
                return MapSolutionToTimetable(optimalSolution, userEmail, timetableName);
            }

            Console.WriteLine("Backtracking failed to find a valid solution. Falling back to greedy algorithm.");
            var greedySolutionFallback = await GreedySchedulingAsync(variables);
            return MapSolutionToTimetable(greedySolutionFallback, userEmail, timetableName);
        }

        private async Task<(Dictionary<Event, List<(Room, Timeslot)>> variables, List<(Event, Room, Timeslot)> currentSolution)> GenerateVariablesWithConstraintsAsync()
        {
            var variables = new Dictionary<Event, List<(Room, Timeslot)>>();
            var professors = await professorRepository.GetAllAsync(userEmail);
            var constraints = new List<Constraint>();

            foreach (var professor in professors.Data)
            {
                var constraintsResult = await constraintRepository.GetConstraintsByProfessorId(professor.Id);
                if (constraintsResult.IsSuccess)
                {
                    constraints.AddRange(constraintsResult.Data);
                }
                else
                {
                    Console.WriteLine($"Error retrieving constraints for professor {professor.Id}: {constraintsResult.ErrorMessage}");
                }
            }

            softConstraints.AddRange(constraints);

            var possibleRooms = (await roomRepository.GetAllAsync(userEmail)).Data ?? new List<Room>();


            var eventTasks = instance.Events.Select(ev =>
            {
                var splitTimeslots = SplitTimeslots(instance.Timeslots, ev.Duration);
                var hardValidator = new HardConstraintValidator(courseRepository, groupRepository);
                var domain = (from room in possibleRooms
                              from timeslot in splitTimeslots
                              where hardValidator.ValidateRoomCapacity(room, ev.EventName)
                              select (room, timeslot)).ToList();

                return Task.FromResult(new { Event = ev, Domain = domain });
            }).ToArray();

            var eventResults = await Task.WhenAll(eventTasks);

            foreach (var result in eventResults)
            {
                variables[result.Event] = result.Domain;
            }

            return (variables, new List<(Event, Room, Timeslot)>());
        }


      
        public async Task<List<(Event, Room, Timeslot)>?> FindOptimalSolutionWithBacktrackingAsync(
            Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            var solution = new List<(Event, Room, Timeslot)>();
            var result = new BacktrackResult();

            var startTime = DateTime.Now;

            await Backtrack(variables, solution, result, startTime);

            return result.isFeasibleSolutionFound ? result.bestSolution : null;
        }

        private async Task Backtrack(
            Dictionary<Event, List<(Room, Timeslot)>> variables,
            List<(Event, Room, Timeslot)> currentSolution,
            BacktrackResult result,
            DateTime startTime)
        {
            if ((DateTime.Now - startTime).TotalMinutes > 2)
            {
                return;
            }

            if (currentSolution.Count == variables.Count)
            {

                // Dacă nu există constrângeri soft, putem accepta orice soluție fezabilă completă
                if (softConstraints.Count == 0)
                {
                    result.isFeasibleSolutionFound = true;
                    result.bestSolution = new List<(Event, Room, Timeslot)>(currentSolution);
                    result.bestScore = 1;
                    return;
                }
                else
                {
                    bool isFeasible = currentSolution.All(ev => IsFeasible(ev.Item1, ev.Item2, ev.Item3, currentSolution));
                    if (isFeasible)
                    {
                        double currentScore = softConstraintsValidator.CalculateTotalScore(currentSolution, softConstraints);
                        if (currentScore > result.bestScore)
                        {
                            result.isFeasibleSolutionFound = true;
                            result.bestSolution = new List<(Event, Room, Timeslot)>(currentSolution);
                            result.bestScore = currentScore;
                        }
                    }
                }
                return;
            }

            var nextEvent = variables
                .Where(kv => !currentSolution.Any(cs => cs.Item1.Id == kv.Key.Id))
                .OrderBy(kv => kv.Value.Count)  // MRV (Minimum Remaining Values)
                .First().Key;

            foreach (var (room, timeslot) in variables[nextEvent])
            {
                if (softConstraints.Count == 0 && result.isFeasibleSolutionFound)
                {
                    return;
                }

                if (!IsFeasible(nextEvent, room, timeslot, currentSolution))
                {
                    continue;
                }

                currentSolution.Add((nextEvent, room, timeslot));
                double currentScore = softConstraintsValidator.CalculateTotalScore(currentSolution, softConstraints);
                double potentialScore = currentScore + softConstraintsValidator.MaxRemainingScore(variables, currentSolution, softConstraints);

                if (softConstraints.Count == 0 || potentialScore > result.bestScore)
                {
                    await Backtrack(variables, currentSolution, result, startTime);
                }

                currentSolution.Remove((nextEvent, room, timeslot));
            }
        }


        private Task<List<(Event, Room, Timeslot)>> GreedySchedulingAsync(Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            var solution = new List<(Event, Room, Timeslot)>();

            foreach (var ev in variables.Keys)
            {
                (Room, Timeslot)? bestFeasibleAssignment = null;

                double bestOverallScore = double.MinValue;
                (Room, Timeslot)? bestOverallAssignment = null;

                foreach (var (room, timeslot) in variables[ev])
                {
                    double score = softConstraintsValidator.CalculateScore(ev, room, timeslot, solution, softConstraints);

                    if (score > bestOverallScore)
                    {
                        bestOverallScore = score;
                        bestOverallAssignment = (room, timeslot);

                        if (IsFeasible(ev, room, timeslot, solution))
                        {
                            bestFeasibleAssignment = (room, timeslot);
                        }
                    }
                }

                if (bestFeasibleAssignment.HasValue)
                {
                    solution.Add((ev, bestFeasibleAssignment.Value.Item1, bestFeasibleAssignment.Value.Item2));
                }
                else if (bestOverallAssignment.HasValue)
                {
                    solution.Add((ev, bestOverallAssignment.Value.Item1, bestOverallAssignment.Value.Item2));
                }
                else
                {
                    if (variables[ev].Any())
                    {
                        var bestAssignment = variables[ev]
                            .OrderByDescending(v => softConstraintsValidator
                            .CalculateScore(ev, v.Item1, v.Item2, solution, softConstraints)).First();
                        solution.Add((ev, bestAssignment.Item1, bestAssignment.Item2));
                    }
                }
            }

            return Task.FromResult(solution);
        }



        private bool IsFeasible(Event ev, Room room, Timeslot timeslot, List<(Event, Room, Timeslot)> currentSolution)
        {
            var hardValidator = new HardConstraintValidator(courseRepository, groupRepository);

            foreach (var (assignedEvent, assignedRoom, assignedTimeslot) in currentSolution)
            {
                if (ev.Id == assignedEvent.Id)
                {
                    continue;
                }

                var roomsAreEqual = room.Id == assignedRoom.Id;
                var professorOverlap = ev.ProfessorId == assignedEvent.ProfessorId;

                var timeslotsOverlap = timeslot.Day == assignedTimeslot.Day &&
                                       hardValidator.TimeslotsOverlap(timeslot, ev.Duration, assignedTimeslot, assignedEvent.Duration);

                if ((roomsAreEqual && timeslotsOverlap) || (professorOverlap && timeslotsOverlap))
                {
                    return false;
                }

                if (!hardValidator.ValidateGroupOverlap(ev, assignedEvent, (room, timeslot), (assignedRoom, assignedTimeslot)))
                {
                    return false;
                }
            }

            return true;
        }

        public List<Timeslot> SplitTimeslots(List<Timeslot> timeslots, int eventDuration)
        {
            if (timeslotCache.TryGetValue(eventDuration, out var cachedTimeslots))
            {
                return cachedTimeslots;
            }

            var eventLength = TimeSpan.FromHours(eventDuration);
            var splitTimeslots = new List<Timeslot>(timeslots.Count * 3);
            var increment = TimeSpan.FromHours(1);


            foreach (var timeslot in timeslots)
            {
                var separatorIndex = timeslot.Time.IndexOf(" - ");
                if (separatorIndex == -1)
                {
                    continue;
                }

                var startTime = TimeSpan.Parse(timeslot.Time[..separatorIndex].Trim());
                var endTime = TimeSpan.Parse(timeslot.Time[(separatorIndex + 3)..].Trim());

                for (var currentTime = startTime; currentTime + eventLength <= endTime; currentTime += increment)
                {
                    var newTimeslot = new Timeslot
                    {
                        Day = timeslot.Day,
                        Time = $"{currentTime:hh\\:mm} - {(currentTime + eventLength):hh\\:mm}"
                    };

                    splitTimeslots.Add(newTimeslot);
                }
            }

            timeslotCache[eventDuration] = splitTimeslots;
            return splitTimeslots;
        }


        public static Timetable MapSolutionToTimetable(List<(Event, Room, Timeslot)> solution, string userEmail, string name)
        {
            var timetable = new Timetable
            {
                Id = Guid.NewGuid(),
                UserEmail = userEmail,
                Name = name
            };

            foreach (var (ev, room, timeslot) in solution)
            {
                ev.RoomId = room.Id;
                ev.Timeslot = new Timeslot
                {
                    Day = timeslot.Day,
                    Time = timeslot.Time
                };
                ev.TimetableId = timetable.Id;
                timetable.Events.Add(ev);
            }

            return timetable;
        }

    }
}
