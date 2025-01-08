using Application.Validators;
using Domain.Entities;
using Domain.Repositories;
using System.Globalization;

namespace Application.Services
{
    public class TimetableGeneratorService
    {
        private readonly string userEmail;
        private readonly Instance instance;
        private readonly IRoomRepository roomRepository;
        private readonly IGroupRepository groupRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IConstraintRepository constraintRepository;
        private readonly IProfessorRepository professorRepository;
        private readonly Guid? timetableId;
        private List<Constraint> softConstraints = new List<Constraint>();

        public TimetableGeneratorService(
            string userEmail,
            Instance instance,
            IRoomRepository roomRepository,
            IGroupRepository groupRepository,
            ICourseRepository courseRepository,
            IConstraintRepository constraintRepository,
            IProfessorRepository professorRepository,
            Guid? timetableId = null)
        {
            this.userEmail = userEmail;
            this.instance = instance;
            this.roomRepository = roomRepository;
            this.groupRepository = groupRepository;
            this.courseRepository = courseRepository;
            this.constraintRepository = constraintRepository;
            this.professorRepository = professorRepository;
            this.timetableId = timetableId;
        }

        public async Task<Timetable> GenerateBestTimetableAsync()
        {
            var (variables, _) = await GenerateVariablesWithConstraintsAsync();

            var greedySolution = await GreedySchedulingAsync(variables);

            return MapSolutionToTimetable(greedySolution);
        }

        private async Task<(Dictionary<Event, List<(Room, Timeslot)>> variables, Dictionary<Event, (Room, Timeslot)> currentSolution)> GenerateVariablesWithConstraintsAsync()
        {
            var variables = new Dictionary<Event, List<(Room, Timeslot)>>();
            //var professors = await professorRepository.GetAllAsync(userEmail);
            var professors = await professorRepository.GetAllAsync("admin@gmail.com"); //pt testare

            foreach (var professor in professors.Data)
            {
                var constraintsResult = await constraintRepository.GetConstraintsByProfessorId(professor.Id);

                if (constraintsResult.IsSuccess)
                {
                    softConstraints.AddRange(constraintsResult.Data);
                }
                else
                {
                    Console.WriteLine($"Error retrieving constraints for professor {professor.Id}: {constraintsResult.ErrorMessage}");
                }
            }

            Console.WriteLine($"Numarul total de constrângeri soft găsite: {softConstraints.Count}");


            Console.WriteLine("Constrangeri soft:");
            foreach (var constraint in softConstraints)
            {
                {
                    Console.WriteLine($"- Id: {constraint.Id}");
                    Console.WriteLine($"  TimetableId: {constraint.TimetableId}");
                    Console.WriteLine($"  Type: {constraint.Type}");
                    Console.WriteLine($"  ProfessorId: {constraint.ProfessorId}");
                    Console.WriteLine($"  CourseId: {constraint.CourseId}");
                    Console.WriteLine($"  RoomId: {constraint.RoomId}");
                    Console.WriteLine($"  WantedRoomId: {constraint.WantedRoomId}");
                    Console.WriteLine($"  GroupId: {constraint.GroupId}");
                    Console.WriteLine($"  Day: {constraint.Day}");
                    Console.WriteLine($"  Time: {constraint.Time}");
                    Console.WriteLine($"  WantedDay: {constraint.WantedDay}");
                    Console.WriteLine($"  WantedTime: {constraint.WantedTime}");
                    Console.WriteLine($"  Event: {constraint.Event}");
                    Console.WriteLine("----------------------------------------");
                }
            }


            foreach (var ev in instance.Events)
            {
                Console.WriteLine($"Processing event: {ev.EventName}, ID: {ev.Id}");

                var possibleRooms = (await roomRepository.GetAllAsync(userEmail)).Data ?? new List<Room>();
                var splitTimeslots = SplitTimeslots(instance.Timeslots, ev.Duration);

                var hardValidator = new HardConstraintValidator(courseRepository, groupRepository);

                var domain = (from room in possibleRooms
                              from timeslot in splitTimeslots
                              where hardValidator.ValidateRoomCapacity(room, ev.EventName)
                              select (room, timeslot)).ToList();

                variables[ev] = domain;
                Console.WriteLine($"Found {domain.Count} possible assignments for event {ev.Id}");
            }

            return (variables, new Dictionary<Event, (Room, Timeslot)>());
        }


        private async Task<Dictionary<Event, (Room, Timeslot)>> GreedySchedulingAsync(
            Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            var solution = new Dictionary<Event, (Room, Timeslot)>();

            foreach (var ev in variables.Keys)
            {
                double bestScore = double.MinValue;
                (Room, Timeslot)? bestAssignment = null;

                foreach (var (room, timeslot) in variables[ev])
                {
                    if (IsFeasible(ev, room, timeslot, solution)) // Validare constrangeri hard
                    {
                        double score = await CalculateScoreAsync(ev, room, timeslot);

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestAssignment = (room, timeslot);
                        }
                    }
                }

                if (bestAssignment.HasValue)
                {
                    solution[ev] = bestAssignment.Value;
                    MarkResourceAsUsed(bestAssignment.Value, solution, variables);
                }
                else
                {
                    Console.WriteLine($"Warning: No valid assignment found for event {ev.EventName}");
                }
            }

            return solution;
        }

        private bool IsFeasible(Event ev, Room room, Timeslot timeslot, Dictionary<Event, (Room, Timeslot)> currentSolution)
        {
            var hardValidator = new HardConstraintValidator(courseRepository, groupRepository);

            foreach (var (assignedEvent, (assignedRoom, assignedTimeslot)) in currentSolution)
            {
                var roomsAreEqual = room.Id == assignedRoom.Id;
                var professorOverlap = ev.ProfessorId == assignedEvent.ProfessorId;

                // Verificăm suprapunerea intervalelor de timp
                var timeslotsOverlap = timeslot.Day == assignedTimeslot.Day &&
                                       hardValidator.TimeslotsOverlap(timeslot, ev.Duration, assignedTimeslot, assignedEvent.Duration);

                // Suprapunere de timp pentru aceeași sală sau profesor
                if ((roomsAreEqual && timeslotsOverlap) || (professorOverlap && timeslotsOverlap))
                {
                    return false;
                }

                // Suprapunere de timp pentru aceeași grupă
                if (!hardValidator.ValidateGroupOverlap(ev, assignedEvent, (room, timeslot), (assignedRoom, assignedTimeslot)))
                {
                    return false;
                }
            }

            return true; // Asignarea este validă dacă nu există conflicte

            //aici se poate creste scorul foarte foarte mult in loc sa returneze true/false,
            //ca sa se returneze totusi o solutie aproximativa daca nu a fost gasita una perfect optima, fara overlapping
        }


        private async Task<double> CalculateScoreAsync(Event ev, Room room, Timeslot timeslot)
        {
            double score = 0;

            foreach (var constraint in softConstraints) { 
            switch (constraint.Type)
            {
                //case ConstraintType.SOFT_ROOM_CHANGE:
                //    if (constraint.ProfessorId == ev.ProfessorId &&
                //        constraint.CourseId == ev.CourseId &&
                //        constraint.GroupId == ev.GroupId &&
                //        constraint.WantedRoomId == room.Id)
                //    {
                //        score += 50;
                //    }
                //    else
                //    {
                //        score -= 20;
                //    }
                //    break;

                case ConstraintType.SOFT_ROOM_PREFERENCE:
                    if (constraint.ProfessorId == ev.ProfessorId && constraint.RoomId == room.Id)
                    {
                        score += 50;
                    }
                    else
                    {
                        score -= 20;
                    }
                    break;

                //case ConstraintType.SOFT_TIME_CHANGE:
                //    if (constraint.ProfessorId == ev.ProfessorId &&
                //        constraint.CourseId == ev.CourseId &&
                //        constraint.GroupId == ev.GroupId &&
                //        constraint.WantedDay == timeslot.Day &&
                //        constraint.WantedTime == timeslot.Time)
                //    {
                //        score += 50;
                //    }
                //    else
                //    {
                //        score -= 20; 
                //    }
                //    break;

                

                case ConstraintType.SOFT_INTERVAL_UNAVAILABILITY:
                    if (constraint.ProfessorId == ev.ProfessorId && constraint.Day == timeslot.Day)
                    {
                        var constraintStart = DateTime.ParseExact(constraint.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                        var constraintEnd = DateTime.ParseExact(constraint.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                        var eventStart = DateTime.ParseExact(timeslot.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                        var eventEnd = DateTime.ParseExact(timeslot.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                        if ((eventStart < constraintEnd && eventEnd > constraintStart)) 
                        {
                            score -= 50; 
                        }
                    }
                    break;


                case ConstraintType.SOFT_INTERVAL_AVAILABILITY:
                    if (constraint.ProfessorId == ev.ProfessorId && constraint.Day == timeslot.Day)
                    {
                        var constraintStart = DateTime.ParseExact(constraint.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                        var constraintEnd = DateTime.ParseExact(constraint.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                        var eventStart = DateTime.ParseExact(timeslot.Time.Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture);
                        var eventEnd = DateTime.ParseExact(timeslot.Time.Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture);

                        if ((eventStart >= constraintStart && eventEnd <= constraintEnd))
                        {
                            score += 50; 
                        }
                    }
                    break;


                    case ConstraintType.SOFT_DAY_OFF:
                    if (constraint.ProfessorId == ev.ProfessorId &&
                        constraint.Day == timeslot.Day)
                    {
                        score -= 50;
                    }
                    break;

                //case ConstraintType.SOFT_ADD_WINDOW:
                //    // Exemplu: Bonus pentru adăugarea unei ferestre de timp adiționale
                //    score += 10;
                //    break;

                //case ConstraintType.SOFT_REMOVE_WINDOW:
                //    // Exemplu: Penalizare pentru eliminarea unei ferestre de timp
                //    score -= 10;
                //    break;

                //case ConstraintType.SOFT_DAY_CHANGE:
                //    // Exemplu: Bonus sau penalizare pentru schimbarea zilei
                //    score += 10;
                //    break;

                //case ConstraintType.SOFT_WEEK_EVENNESS:
                //    // Exemplu: Bonus pentru o distribuție echitabilă pe zilele săptămânii
                //    score += 20;
                //    break;

            }
        }

            return score;
        } 


        private void MarkResourceAsUsed(
            (Room room, Timeslot timeslot) assignment,
            Dictionary<Event, (Room, Timeslot)> solution,
            Dictionary<Event, List<(Room, Timeslot)>> variables)
        {
            var (room, timeslot) = assignment;

            foreach (var ev in variables.Keys)
            {
                variables[ev].RemoveAll(x => x.Item1 == room && x.Item2 == timeslot);
            }
        }


        private Timetable MapSolutionToTimetable(Dictionary<Event, (Room, Timeslot)> solution)
        {
            var timetable = new Timetable { Id = Guid.NewGuid() };

            foreach (var (ev, value) in solution)
            {
                var (room, timeslot) = value;
                ev.RoomId = room.Id;
                ev.Timeslot = timeslot;
                ev.TimetableId = timetable.Id;
                timetable.Events.Add(ev);
            }

            return timetable;
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

    }
}