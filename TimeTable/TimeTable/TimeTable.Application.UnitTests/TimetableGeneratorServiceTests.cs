using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

public class TimetableGeneratorServiceTests
{
    private readonly IRoomRepository _roomRepository = Substitute.For<IRoomRepository>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly ICourseRepository _courseRepository = Substitute.For<ICourseRepository>();
    private readonly IConstraintRepository _constraintRepository = Substitute.For<IConstraintRepository>();
    private readonly IProfessorRepository _professorRepository = Substitute.For<IProfessorRepository>();
    private readonly Instance _instance = new Instance
    {
        Events = new List<Event>
        {
            new Event
            {
                Id = Guid.NewGuid(),
                EventName = "Math Lecture",
                Duration = 2,
                ProfessorId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            }
        },
        Timeslots = new List<Timeslot>
        {
            new Timeslot { Day = "Monday", Time = "08:00 - 10:00" },
            new Timeslot { Day = "Monday", Time = "10:00 - 12:00" }
        }
    };

    [Fact]
    public async Task Given_TimetableGeneratorService_When_OptimalSolutionExists_Then_ReturnOptimalTimetable()
    {
        // Arrange
        _professorRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(new List<Professor>()));
        _roomRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(new List<Room>
        {
            new Room { Id = Guid.NewGuid(), Name = "Room 1", Capacity = 30, UserEmail = "user@example.com" }
        }));
        _constraintRepository.GetConstraintsByProfessorId(Arg.Any<Guid>())
            .Returns(Result<IEnumerable<Constraint>>.Success(new List<Constraint>()));

        var service = new TimetableGeneratorService(
            "user@example.com",
            _instance,
            _roomRepository,
            _groupRepository,
            _courseRepository,
            _constraintRepository,
            _professorRepository,
            "Sample Timetable");

        // Act
        var timetable = await service.GenerateBestTimetableAsync();

        // Assert
        Assert.NotNull(timetable);
        Assert.Equal("Sample Timetable", timetable.Name);
        Assert.NotEmpty(timetable.Events);
    }

    //[Fact]
    //public async Task Given_TimetableGeneratorService_When_BacktrackingFails_Then_FallbackToGreedyAlgorithm()
    //{
    //    // Arrange
    //    _professorRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(new List<Professor>()));
    //    _roomRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(new List<Room>
    //{
    //    new Room { Id = Guid.NewGuid(), Name = "Room 1", Capacity = 30, UserEmail = "user@example.com" }
    //}));
    //    _constraintRepository.GetConstraintsByProfessorId(Arg.Any<Guid>())
    //        .Returns(Result<IEnumerable<Constraint>>.Success(new List<Constraint>()));

    //    // Creează un substitut complet pentru serviciu
    //    var service = Substitute.For<TimetableGeneratorService>(
    //        "user@example.com",
    //        _instance,
    //        _roomRepository,
    //        _groupRepository,
    //        _courseRepository,
    //        _constraintRepository,
    //        _professorRepository,
    //        "Sample Timetable"
    //    );

    //    // Mock metodele pentru a simula comportamente
    //    service.When(x => x.FindOptimalSolutionWithBacktrackingAsync(Arg.Any<Dictionary<Event, List<(Room, Timeslot)>>>())).DoNotCallBase();
    //    service.FindOptimalSolutionWithBacktrackingAsync(Arg.Any<Dictionary<Event, List<(Room, Timeslot)>>>()).Returns((List<(Event, Room, Timeslot)>)null);

    //    // Act
    //    var timetable = await service.GenerateBestTimetableAsync();

    //    // Assert
    //    Assert.NotNull(timetable);
    //    Assert.Equal("Sample Timetable", timetable.Name);
    //    Assert.NotEmpty(timetable.Events);
    //}



    [Fact]
    public async Task Given_TimetableGeneratorService_When_NoConstraintsArePresent_Then_ReturnValidTimetable()
    {
        // Arrange
        _professorRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(new List<Professor>()));
        _roomRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(new List<Room>
        {
            new Room { Id = Guid.NewGuid(), Name = "Room 1", Capacity = 30, UserEmail = "user@example.com" }
        }));

        _constraintRepository.GetConstraintsByProfessorId(Arg.Any<Guid>())
            .Returns(Result<IEnumerable<Constraint>>.Success(new List<Constraint>()));

        var service = new TimetableGeneratorService(
            "user@example.com",
            _instance,
            _roomRepository,
            _groupRepository,
            _courseRepository,
            _constraintRepository,
            _professorRepository,
            "Sample Timetable");

        // Act
        var timetable = await service.GenerateBestTimetableAsync();

        // Assert
        Assert.NotNull(timetable);
        Assert.Equal("Sample Timetable", timetable.Name);
        Assert.NotEmpty(timetable.Events);
    }

    [Fact]
    public async Task Given_TimetableGeneratorService_When_ConstraintsExist_Then_ValidateAgainstConstraints()
    {
        // Arrange
        var professorId = Guid.NewGuid();
        _professorRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(new List<Professor>
    {
        new Professor { Id = professorId, Name = "Example Name", UserEmail = "user@test.com" }
    }));
        _constraintRepository.GetConstraintsByProfessorId(professorId)
            .Returns(Result<IEnumerable<Constraint>>.Success(new List<Constraint>
            {
            new Constraint
            {
                Type = ConstraintType.SOFT_INTERVAL_UNAVAILABILITY,
                ProfessorId = professorId,
                Day = "Monday",
                Time = "08:00 - 10:00"
            }
            }));
        _roomRepository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(new List<Room>
    {
        new Room { Id = Guid.NewGuid(), Name = "Room 1", Capacity = 30, UserEmail = "user@example.com" }
    }));

        var _instance = new Instance
        {
            Events = new List<Event>
        {
            new Event
            {
                Id = Guid.NewGuid(),
                EventName = "laboratory",
                CourseId = Guid.NewGuid(),
                ProfessorId = professorId,
                GroupId = Guid.NewGuid(),
                Duration = 2,
                IsEven = false
            },
            new Event
            {
                Id = Guid.NewGuid(),
                EventName = "course",
                CourseId = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                Duration = 1,
                IsEven = false
            }
        },
            Timeslots = new List<Timeslot>
        {
            new Timeslot { Day = "Monday", Time = "08:00 - 09:00" },
            new Timeslot { Day = "Monday", Time = "09:00 - 10:00" },
            new Timeslot { Day = "Monday", Time = "10:00 - 11:00" },
            new Timeslot { Day = "Monday", Time = "11:00 - 12:00" },
            new Timeslot { Day = "Tuesday", Time = "08:00 - 09:00" }
        }
        };

        var service = new TimetableGeneratorService(
            "user@example.com",
            _instance,
            _roomRepository,
            _groupRepository,
            _courseRepository,
            _constraintRepository,
            _professorRepository,
            "Sample Timetable");

        // Act
        var timetable = await service.GenerateBestTimetableAsync();

        // Assert
        Assert.NotNull(timetable);
        Assert.All(timetable.Events, ev =>
        {
            Assert.NotEqual("Monday", ev.Timeslot.Day);
        });
    }

    [Fact]
    public async Task Given_TimetableGeneratorService_When_ValidEventsAndConstraints_Then_ReturnNonNullTimetable()
    {
        // Arrange
        var professorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        _professorRepository.GetAllAsync(Arg.Any<string>())
            .Returns(Result<IEnumerable<Professor>>.Success(new List<Professor>
            {
            new Professor { Id = professorId, Name = "Dr. Smith", UserEmail = "dr.smith@example.com" }
            }));

        _roomRepository.GetAllAsync(Arg.Any<string>())
            .Returns(Result<IEnumerable<Room>>.Success(new List<Room>
            {
            new Room { Id = roomId, Name = "Room 1", Capacity = 30, UserEmail = "user@example.com" }
            }));

        _constraintRepository.GetConstraintsByProfessorId(professorId)
            .Returns(Result<IEnumerable<Constraint>>.Success(new List<Constraint>
            {
                // No constraints are violated in this case
            }));

        // Simulating events that are scheduled at different times and have no conflicts
        var _instance = new Instance
        {
            Events = new List<Event>
        {
            new Event
            {
                Id = Guid.NewGuid(),
                EventName = "seminary",
                Duration = 2,
                ProfessorId = professorId,
                GroupId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
            },
            new Event
            {
                Id = Guid.NewGuid(),
                EventName = "course",
                Duration = 2,
                ProfessorId = professorId,
                GroupId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
            }
        },
            Timeslots = new List<Timeslot>
        {
            new Timeslot { Day = "Monday", Time = "08:00 - 10:00" },
            new Timeslot { Day = "Monday", Time = "10:00 - 12:00" }
        }
        };

        var service = new TimetableGeneratorService(
            "user@example.com",
            _instance,
            _roomRepository,
            _groupRepository,
            _courseRepository,
            _constraintRepository,
            _professorRepository,
            "Sample Timetable");

        // Act
        var timetable = await service.GenerateBestTimetableAsync();

        // Assert
        Assert.NotNull(timetable); // The timetable should not be null since there are no conflicts
        Assert.Equal("Sample Timetable", timetable.Name); // Ensure that the timetable has the expected name
        Assert.NotEmpty(timetable.Events); // Ensure that the timetable contains events
        Assert.All(timetable.Events, ev =>
        {
            Assert.NotNull(ev.Timeslot); // Ensure each event has a valid timeslot assigned
            Assert.NotNull(ev.RoomId); // Ensure that each event has a room assigned
            Assert.NotNull(ev.ProfessorId); // Ensure that each event has a professor assigned
        });
    }

}
