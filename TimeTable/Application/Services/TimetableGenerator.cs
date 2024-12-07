using Application.Validators;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services
{
    public class TimetableGenerator(
        string userEmail,
        Instance instance,
        IRoomRepository roomRepository,
        IGroupRepository groupRepository,
        IProfessorRepository professorRepository,
        ICourseRepository courseRepository,
        IConstraintRepository constraintRepository,
        Guid? timetableId = null)
    {
        private readonly IGroupRepository _groupRepository = groupRepository;

        public Timetable GenerateBestTimetable(out Dictionary<Event, (Room, Timeslot)> bestSolution)
        {
            // Generate domains for events based on constraints
            var variables = GenerateVariablesWithConstraints();

            // Evaluate all possible timetables
            var candidateSolutions = GenerateAllPossibleTimetables(variables);

            // Score the timetables
            var scoredSolutions = ScoreTimetables(candidateSolutions);

            // Select the best timetable
            var bestScoredSolution = scoredSolutions.OrderByDescending(s => s.Score).FirstOrDefault();

            if (bestScoredSolution == null)
            {
                bestSolution = null!;
                throw new Exception("No valid timetable could be generated.");
            }

            bestSolution = bestScoredSolution.Assignment;
            return MapSolutionToTimetable(bestSolution);
        }

        private Dictionary<Event, List<(Room, Timeslot)>> GenerateVariablesWithConstraints()
        {
            var variables = new Dictionary<Event, List<(Room, Timeslot)>>();

            foreach (var ev in instance.Events)
            {
                var possibleRooms = roomRepository.GetAllAsync(userEmail).Result?.Data ?? new List<Room>();
                var splitTimeslots = SplitTimeslots(instance.Timeslots, ev.TimeInterval);
                HardConstraintValidator hardValidator = new(courseRepository);
                
                var domain = (from room in possibleRooms
                    from timeslot in splitTimeslots
                    where hardValidator.ValidateRoomCapacity(room, ev.EventName)
                    select (room, timeslot)).ToList();

                variables[ev] = domain;
            }

            return variables;
        }

        private List<Timeslot> SplitTimeslots(List<Timeslot> timeslots, int eventDuration)
        {
            var splitTimeslots = new List<Timeslot>();
            var eventLength = TimeSpan.FromHours(eventDuration);

            foreach (var timeslot in timeslots)
            {
                // Parse the Time property (e.g., "08:00 - 10:00")
                var timeParts = timeslot.Time.Split(" - ");
                var startTime = TimeSpan.Parse(timeParts[0]);
                var endTime = TimeSpan.Parse(timeParts[1]);

                // Split into smaller intervals
                while (startTime + eventLength <= endTime)
                {
                    var newTimeslot = new Timeslot
                    {
                        Day = timeslot.Day,
                        Time = $@"{startTime:hh\:mm} - {(startTime + eventLength):hh\:mm}"
                    };

                    splitTimeslots.Add(newTimeslot);
                    startTime += eventLength; // Move to the next interval
                }
            }

            return splitTimeslots;
        }


        private bool ValidateHardConstraints(Event ev, Room room, Timeslot timeslot, Dictionary<Event, (Room, Timeslot)> currentSolution)
        {
            var hardValidator = new HardConstraintValidator(courseRepository);

            // Validate the basic constraints
            if (!hardValidator.ValidateRoomCapacity(room, ev.EventName))
                return false;

            // Validate NoOverlap and GroupOverlap constraints
            foreach (var (assignedEvent, (assignedRoom, assignedTimeslot)) in currentSolution)
            {
                if (!hardValidator.ValidateNoOverlap(ev, assignedEvent, (room, timeslot), (assignedRoom, assignedTimeslot)) ||
                    !hardValidator.ValidateGroupOverlap(ev, assignedEvent, (room, timeslot), (assignedRoom, assignedTimeslot)))
                {
                    return false;
                }
            }

            return true;
        }


        private List<TimetableSolution> GenerateAllPossibleTimetables(Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            var solutions = new List<TimetableSolution>();
            GenerateSolutionsRecursive(variables, new Dictionary<Event, (Room, Timeslot)>(), solutions);
            return solutions;
        }

        private void GenerateSolutionsRecursive(
            Dictionary<Event, List<(Room, Timeslot)>> variables,
            Dictionary<Event, (Room, Timeslot)> currentSolution,
            List<TimetableSolution> solutions)
        {
            if (currentSolution.Count == variables.Count)
            {
                solutions.Add(new TimetableSolution
                {
                    Assignment = new Dictionary<Event, (Room, Timeslot)>(currentSolution),
                    Score = 0 // Will be computed later
                });
                return;
            }

            var unassigned = variables.Keys.First(ev => !currentSolution.ContainsKey(ev));
            foreach (var value in variables[unassigned].Where(value => IsAssignmentConsistent(unassigned, value, currentSolution)))
            {
                currentSolution[unassigned] = value;
                GenerateSolutionsRecursive(variables, currentSolution, solutions);
                currentSolution.Remove(unassigned);
            }
        }

        private bool IsAssignmentConsistent(Event ev, (Room, Timeslot) value, Dictionary<Event, (Room, Timeslot)> assignment)
        {
            return ValidateHardConstraints(ev, value.Item1, value.Item2, assignment);
        }


        private List<ScoredTimetable> ScoreTimetables(List<TimetableSolution> solutions)
        {
            var scoredSolutions = new List<ScoredTimetable>();
            const int hardWeight = 10;
            const int softWeight = 1;

            foreach (var solution in solutions)
            {
                var hardScore = 0;
                var softScore = 0;

                // Iterate through the assignments to evaluate constraints
                foreach (var (currentEvent, currentAssignment) in solution.Assignment)
                {
                    var (room, timeslot) = currentAssignment;

                    // Validate hard constraints in the context of the full assignment
                    if (ValidateHardConstraints(currentEvent, room, timeslot, solution.Assignment))
                    {
                        hardScore += hardWeight;
                    }

                    // Validate soft constraints (assumes these are independent of other assignments)
                    softScore += ValidateSoftConstraints(currentEvent, room, timeslot);
                }

                // Add the scored solution
                scoredSolutions.Add(new ScoredTimetable
                {
                    Assignment = solution.Assignment,
                    Score = hardScore + (softScore * softWeight)
                });
            }

            return scoredSolutions;
        }


        private int ValidateSoftConstraints(Event ev, Room room, Timeslot timeslot)
        {
            if( timetableId == null)
            {
                return 0;
            }
            
            var softValidator = new SoftConstraintValidator(instance);

            var constraints = constraintRepository.GetAllAsync(timetableId.Value).Result?.Data ?? new List<Constraint>();

            return constraints.Count(constraint => softValidator.Validate(constraint, ev, (room, timeslot)));
        }

        private Timetable MapSolutionToTimetable(Dictionary<Event, (Room, Timeslot)> solution)
        {
            var timetable = new Timetable { Id = Guid.NewGuid() };

            foreach (var (ev, value) in solution)
            {
                var (room, timeslot) = value;

                var course = courseRepository.GetByIdAsync(ev.CourseId).Result?.Data;
                ev.CoursePackage = course?.Package ?? "";
                ev.CourseCredits = course?.Credits ?? 0;
                ev.ProfessorName = professorRepository.GetByIdAsync(ev.ProfessorId).Result?.Data?.Name ?? "";
                timeslot.Event = ev;
                timeslot.RoomName = room.Name;
                timeslot.TimetableId = timetable.Id;
                timetable.Timeslots.Add(timeslot);
            }

            return timetable;
        }
    }

    public class TimetableSolution
    {
        public Dictionary<Event, (Room, Timeslot)> Assignment { get; init; } = new();
        public int Score { get; set; }
    }

    public class ScoredTimetable
    {
        public Dictionary<Event, (Room, Timeslot)> Assignment { get; init; } = new();
        public int Score { get; init; }
    }
}
