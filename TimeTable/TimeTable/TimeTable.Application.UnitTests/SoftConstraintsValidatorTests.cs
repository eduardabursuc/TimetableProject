using Application.Validators;
using Domain.Entities;
using Domain.Common;
using Xunit;

namespace TimeTable.Application.UnitTests
{
    public class SoftConstraintsValidatorTests
    {
        private readonly SoftConstraintsValidator validator;

        public SoftConstraintsValidatorTests()
        {
            validator = new SoftConstraintsValidator();
        }

        [Fact]
        public void CalculateScore_WithSoftRoomChangeConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = ev.ProfessorId,
                    CourseId = ev.CourseId,
                    GroupId = ev.GroupId,
                    WantedRoomId = room.Id,
                    Day = "Monday",
                    Time = "08:00 - 10:00"
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the room change constraint is added
        }

        [Fact]
        public void CalculateScore_WithSoftTimeChangeConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "seminary",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_TIME_CHANGE,
                    ProfessorId = ev.ProfessorId,
                    CourseId = ev.CourseId,
                    GroupId = ev.GroupId,
                    WantedTime = "08:00 - 10:00"
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the time change constraint is added
        }

        [Fact]
        public void CalculateScore_WithSoftDayOffConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "laboratory",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_DAY_OFF,
                    ProfessorId = ev.ProfessorId,
                    Day = "Tuesday" // Professor wants to have the day off on Tuesday
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the day off constraint is added
        }

        [Fact]
        public void CalculateTotalScore_WithMultipleEvents_ReturnsCorrectTotalScore()
        {
            // Arrange
            var ev1 = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var ev2 = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "laboratory",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot1 = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var timeslot2 = new Timeslot
            {
                Day = "Tuesday",
                Time = "10:00 - 12:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = ev1.ProfessorId,
                    CourseId = ev1.CourseId,
                    GroupId = ev1.GroupId,
                    WantedRoomId = room.Id,
                    Day = "Monday",
                    Time = "08:00 - 10:00"
                },
                new Constraint
                {
                    Type = ConstraintType.SOFT_DAY_OFF,
                    ProfessorId = ev2.ProfessorId,
                    Day = "Wednesday"
                }
            };

            var solution = new List<(Event, Room, Timeslot)>
            {
                (ev1, room, timeslot1),
                (ev2, room, timeslot2)
            };

            // Act
            double totalScore = validator.CalculateTotalScore(solution, softConstraints);

            // Assert
            Assert.Equal(100, totalScore); // Ensure that total score for both events is added correctly
        }

        [Fact]
        public void CheckLectureBeforeLab_LectureBeforeLab_ReturnsTrue()
        {
            // Arrange
            var evLecture = new Event
            {
                Id = Guid.NewGuid(),
                EventName = "course",
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                Duration = 2
            };

            var evLab = new Event
            {
                Id = Guid.NewGuid(),
                EventName = "laboratory",
                CourseId = evLecture.CourseId,
                GroupId = evLecture.GroupId,
                ProfessorId = evLecture.ProfessorId,
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslotLecture = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var timeslotLab = new Timeslot
            {
                Day = "Monday",
                Time = "10:00 - 12:00"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
    {
        (evLecture, room, timeslotLecture)
    };

            double score = 0;

            // Act
            bool result = SoftConstraintsValidator.CheckLectureBeforeLab(evLab, timeslotLab, currentSolution, ref score);

            // Assert
            Assert.True(result); // Ensure that lecture is correctly before lab
        }


        [Fact]
        public void CheckLectureBeforeLab_LabBeforeLecture_ReturnsFalse()
        {
            // Arrange
            var evLecture = new Event
            {
                Id = Guid.NewGuid(),
                EventName = "course",
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                Duration = 2
            };

            var evLab = new Event
            {
                Id = Guid.NewGuid(),
                EventName = "laboratory",
                CourseId = evLecture.CourseId,
                GroupId = evLecture.GroupId,
                ProfessorId = evLecture.ProfessorId,
                Duration = 2

            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslotLecture = new Timeslot
            {
                Day = "Monday",
                Time = "10:00 - 12:00"
            };

            var timeslotLab = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
    {
        (evLab, room, timeslotLab)
    };

            double score = 0;

            // Act
            bool result = SoftConstraintsValidator.CheckLectureBeforeLab(evLecture, timeslotLecture, currentSolution, ref score);

            // Assert
            Assert.False(result); // Ensure that lab is correctly not before lecture
        }


        [Fact]
        public void CalculateTotalScore_WithSingleEvent_ReturnsCorrectScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = ev.ProfessorId,
                    CourseId = ev.CourseId,
                    GroupId = ev.GroupId,
                    WantedRoomId = room.Id,
                    Day = "Monday",
                    Time = "08:00 - 10:00"
                }
            };

            var solution = new List<(Event, Room, Timeslot)>
            {
                (ev, room, timeslot)
            };

            // Act
            double totalScore = validator.CalculateTotalScore(solution, softConstraints);

            // Assert
            Assert.Equal(50, totalScore); // Ensure that the score for the single event is added correctly
        }

        [Fact]
        public void CalculateTotalScore_WithMultipleConstraintsForSameEvent_ReturnsCorrectTotalScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = ev.ProfessorId,
                    CourseId = ev.CourseId,
                    GroupId = ev.GroupId,
                    WantedRoomId = room.Id,
                    Day = "Monday",
                    Time = "08:00 - 10:00"
                },
                new Constraint
                {
                    Type = ConstraintType.SOFT_TIME_CHANGE,
                    ProfessorId = ev.ProfessorId,
                    CourseId = ev.CourseId,
                    GroupId = ev.GroupId,
                    WantedTime = "08:00 - 10:00"
                }
            };

            var solution = new List<(Event, Room, Timeslot)>
            {
                (ev, room, timeslot)
            };

            // Act
            double totalScore = validator.CalculateTotalScore(solution, softConstraints);

            // Assert
            Assert.Equal(100, totalScore); // Ensure that the score for the event with multiple constraints is added correctly
        }



        [Fact]
        public void CalculateScore_WithNoSoftConstraints_ReturnsZero()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>(); // No constraints

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(0, score); // Ensure no score is added for no constraints
        }

        [Fact]
        public void CalculateScore_WithSoftTimeChangeConstraintAndConflict_AddsZeroScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "seminary",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_TIME_CHANGE,
                    ProfessorId = ev.ProfessorId,
                    CourseId = ev.CourseId,
                    GroupId = ev.GroupId,
                    WantedTime = "10:00 - 12:00"
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(0, score); // Ensure no score for time change constraint conflict
        }

        [Fact]
        public void CalculateScore_WithSoftDayOffConstraint_ProfessorNotOff_ReturnsZero()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "seminary",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_DAY_OFF,
                    ProfessorId = ev.ProfessorId,
                    Day = "Monday" // Professor doesn't have the day off on Monday
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(0, score); // The constraint is applied - the professor 
        }

        [Fact]
        public void CalculateTotalScore_WithNoEvents_ReturnsZero()
        {
            // Arrange
            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = Guid.NewGuid(),
                    CourseId = Guid.NewGuid(),
                    GroupId = Guid.NewGuid(),
                    WantedRoomId = Guid.NewGuid(),
                    Day = "Monday",
                    Time = "08:00 - 10:00"
                }
            };

            var solution = new List<(Event, Room, Timeslot)>(); // No events

            // Act
            double totalScore = validator.CalculateTotalScore(solution, softConstraints);

            // Assert
            Assert.Equal(0, totalScore); // Ensure no total score when no events exist
        }

        [Fact]
        public void CalculateScore_WithSoftConstraints_ReturnsCorrectScoreForMultipleEventTypes()
        {
            // Arrange
            var ev1 = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "lecture",
                Duration = 2
            };

            var ev2 = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "lab",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot1 = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var timeslot2 = new Timeslot
            {
                Day = "Monday",
                Time = "10:00 - 12:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = ev1.ProfessorId,
                    CourseId = ev1.CourseId,
                    GroupId = ev1.GroupId,
                    WantedRoomId = room.Id,
                    Day = "Monday",
                    Time = "08:00 - 10:00"
                },
                new Constraint
                {
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = ev2.ProfessorId,
                    CourseId = ev2.CourseId,
                    GroupId = ev2.GroupId,
                    WantedRoomId = room.Id,
                    Day = "Monday",
                    Time = "10:00 - 12:00"
                }
            };

            var solution = new List<(Event, Room, Timeslot)>
            {
                (ev1, room, timeslot1),
                (ev2, room, timeslot2)
            };

            // Act
            double totalScore = validator.CalculateTotalScore(solution, softConstraints);

            // Assert
            Assert.Equal(100, totalScore); // Ensure that multiple event types are handled correctly with constraints
        }


        [Fact]
        public void CalculateScore_WithSoftRemoveWindowConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_REMOVE_WINDOW,
                    ProfessorId = ev.ProfessorId,
                    Day = "Monday"
                }
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
            {
                (new Event { Id = Guid.NewGuid(), ProfessorId = ev.ProfessorId, CourseId = Guid.NewGuid(), GroupId = ev.GroupId, EventName = "course", Duration = 2 }, room, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" })
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the remove window constraint is added
        }

        [Fact]
        public void CalculateScore_WithSoftWeekEvennessConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_WEEK_EVENNESS,
                    ProfessorId = ev.ProfessorId,
                    CourseId = ev.CourseId,
                    GroupId = ev.GroupId
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the week evenness constraint is added
        }

        [Fact]
        public void CalculateScore_WithSoftConsecutiveHoursConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_CONSECUTIVE_HOURS,
                    ProfessorId = ev.ProfessorId,
                    Day = "Monday"
                }
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
            {
                (new Event { Id = Guid.NewGuid(), ProfessorId = ev.ProfessorId, CourseId = Guid.NewGuid(), GroupId = ev.GroupId, EventName = "course", Duration = 2 }, room, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" })
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the consecutive hours constraint is added
        }

        [Fact]
        public void CalculateScore_WithSoftIntervalAvailabilityConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "09:00 - 11:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_INTERVAL_AVAILABILITY,
                    ProfessorId = ev.ProfessorId,
                    Day = "Monday",
                    Time = "08:00 - 12:00"
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the interval availability constraint is added
        }

        [Fact]
        public void CalculateScore_WithSoftIntervalUnavailabilityConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "09:00 - 11:00"
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_INTERVAL_UNAVAILABILITY,
                    ProfessorId = ev.ProfessorId,
                    Day = "Monday",
                    Time = "12:00 - 14:00"
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, new List<(Event, Room, Timeslot)>(), softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the interval unavailability constraint is added
        }

        [Fact]
        public void CalculateScore_WithSoftLectureBeforeLabsConstraint_AddsScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                EventName = "laboratory",
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                Duration = 2
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 101",
                Capacity = 100,
                UserEmail = "test@gmail.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "12:00 - 14:00"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
            {
                (ev, room, timeslot),
                (new Event { Id = Guid.NewGuid(), EventName = "course", CourseId = ev.CourseId, GroupId = ev.GroupId, Duration = 2, ProfessorId = ev.ProfessorId }, room, new Timeslot { Day = "Monday", Time = "08:00 - 10:00" })
            };

            var softConstraints = new List<Constraint>
            {
                new Constraint
                {
                    Type = ConstraintType.SOFT_LECTURE_BEFORE_LABS,
                    ProfessorId = ev.ProfessorId,
                }
            };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score); // Ensure that score for the lecture before labs constraint is added
        }


        [Fact]
        public void CalculateScore_SoftDayChangeConstraint_MatchingConstraint_IncreasesScore()
        {
            // Arrange
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                EventName = "course",
                Duration = 2
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var constraint = new Constraint
            {
                Type = ConstraintType.SOFT_DAY_CHANGE,
                ProfessorId = ev.ProfessorId,
                CourseId = ev.CourseId,
                GroupId = ev.GroupId,
                WantedDay = "Monday",
                WantedTime = "08:00 - 10:00"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>();
            var softConstraints = new List<Constraint> { constraint };

            // Act
            double score = validator.CalculateScore(ev, null, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score);
        }

        [Fact]
        public void CalculateScore_SoftAddWindowConstraint_NoBreak_ScoreRemainsZero()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                GroupId = groupId,
                ProfessorId = professorId,
                EventName = "course",
                Duration = 2
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var constraint = new Constraint
            {
                Type = ConstraintType.SOFT_ADD_WINDOW,
                ProfessorId = professorId,
                Day = "Monday"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
    {
        (new Event { ProfessorId = professorId, CourseId = courseId, GroupId = groupId, EventName = "course", Duration = 2 }, null, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" }),
        (new Event { ProfessorId = professorId, CourseId = courseId, GroupId = groupId, EventName = "course", Duration = 2 }, null, new Timeslot { Day = "Monday", Time = "12:00 - 14:00" })
    };

            var softConstraints = new List<Constraint> { constraint };

            // Act
            double score = validator.CalculateScore(ev, null, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(0, score);
        }



        [Fact]
        public void CalculateScore_SoftConsecutiveHoursConstraint_AllConsecutive_IncreasesScore()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = professorId,
                EventName = "course",
                GroupId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                Duration = 2
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var constraint = new Constraint
            {
                Type = ConstraintType.SOFT_CONSECUTIVE_HOURS,
                ProfessorId = professorId,
                Day = "Monday"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
    {
        (new Event { ProfessorId = professorId, CourseId = Guid.NewGuid(), EventName = "course", Duration = 2, GroupId = Guid.NewGuid() },  new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 2"}, new Timeslot { Day = "Monday", Time = "06:00 - 08:00" }),
        (ev,  new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 3"}, timeslot),
        (new Event { ProfessorId = professorId, CourseId = Guid.NewGuid(), GroupId = Guid.NewGuid(), EventName = "seminary", Duration = 2 }, new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 1"}, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" })
    };

            var softConstraints = new List<Constraint> { constraint };

            // Act
            double score = validator.CalculateScore(ev, null, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score);
        }


        [Fact]
        public void CalculateScore_SoftLectureBeforeLabConstraint_LectureBeforeLab_IncreasesScore()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var professorId = Guid.NewGuid();

            var lectureEvent = new Event
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                GroupId = groupId,
                ProfessorId = professorId,
                EventName = "course",
                Duration = 2
            };

            var labEvent = new Event
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                GroupId = groupId,
                ProfessorId = professorId,
                EventName = "laboratory",
                Duration = 2
            };

            var timeslotLecture = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var timeslotLab = new Timeslot
            {
                Day = "Monday",
                Time = "10:00 - 12:00"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
        {
            (lectureEvent, null, timeslotLecture)
        };

            var softConstraints = new List<Constraint>
        {
            new Constraint { Type = ConstraintType.SOFT_LECTURE_BEFORE_LABS, CourseId = courseId }
        };

            // Act
            double score = validator.CalculateScore(labEvent, null, timeslotLab, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score);
        }


    [Fact]
        public void CalculateScore_SoftRoomPreferenceConstraint_IncreasesScore()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var wantedRoomId = Guid.NewGuid();

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ProfessorId = professorId,
                GroupId = Guid.NewGuid(),
                CourseId = courseId,
                EventName = "course",
                Duration = 2
            };

            var room = new Room
            {
                Id = wantedRoomId,
                Name = "Room A",
                Capacity = 100,
                UserEmail = "test@example.com"
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var constraint = new Constraint
            {
                Type = ConstraintType.SOFT_ROOM_PREFERENCE,
                ProfessorId = professorId,
                WantedRoomId = wantedRoomId
            };

            var currentSolution = new List<(Event, Room, Timeslot)>();
            var softConstraints = new List<Constraint> { constraint };

            // Act
            double score = validator.CalculateScore(ev, room, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score);
        }

        [Fact]
        public void CalculateScore_SoftAddWindowConstraint_WithBreak_IncreasesScore()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                GroupId = groupId,
                ProfessorId = professorId,
                EventName = "course",
                Duration = 2
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var constraint = new Constraint
            {
                Type = ConstraintType.SOFT_ADD_WINDOW,
                ProfessorId = professorId,
                Day = "Monday"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
    {
         (new Event { ProfessorId = professorId, CourseId = Guid.NewGuid(), EventName = "course", Duration = 2, GroupId = Guid.NewGuid() },  new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 2"}, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" }),
         (new Event { ProfessorId = professorId, CourseId = Guid.NewGuid(), EventName = "course", Duration = 2, GroupId = Guid.NewGuid() },  new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 2"}, new Timeslot { Day = "Monday", Time = "14:00 - 16:00" })
    };

            var softConstraints = new List<Constraint> { constraint };

            // Act
            double score = validator.CalculateScore(ev, null, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score);
        }

        [Fact]
        public void CalculateScore_SoftConsecutiveHoursConstraint_NotAllConsecutive_ScoreRemainsZero()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                GroupId = groupId,
                ProfessorId = professorId,
                EventName = "course",
                Duration = 2
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var constraint = new Constraint
            {
                Type = ConstraintType.SOFT_CONSECUTIVE_HOURS,
                ProfessorId = professorId,
                Day = "Monday"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
            {
                (new Event { ProfessorId = professorId, CourseId = Guid.NewGuid(), EventName = "course", Duration = 2, GroupId = Guid.NewGuid() },  new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 2"}, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" }),
                (new Event { ProfessorId = professorId, CourseId = Guid.NewGuid(), EventName = "course", Duration = 2, GroupId = Guid.NewGuid() },  new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 2"}, new Timeslot { Day = "Monday", Time = "14:00 - 16:00" })
            };

            var softConstraints = new List<Constraint> { constraint };

            // Act
            double score = validator.CalculateScore(ev, null, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(0, score);
        }

        [Fact]
        public void CalculateScore_SoftRemoveWindowConstraint_WithNoBreak_IncreasesScore()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                GroupId = groupId,
                ProfessorId = professorId,
                EventName = "course",
                Duration = 2
            };

            var timeslot = new Timeslot
            {
                Day = "Monday",
                Time = "08:00 - 10:00"
            };

            var constraint = new Constraint
            {
                Type = ConstraintType.SOFT_REMOVE_WINDOW,
                ProfessorId = professorId,
                Day = "Monday"
            };

            var currentSolution = new List<(Event, Room, Timeslot)>
            {
                (new Event { ProfessorId = professorId, CourseId = Guid.NewGuid(), EventName = "course", Duration = 2, GroupId = Guid.NewGuid() },  new Room{UserEmail = "prof@test.com", Capacity = 100, Name = "Room 2"}, new Timeslot { Day = "Monday", Time = "12:00 - 14:00" })
            };
            var softConstraints = new List<Constraint> { constraint };

            // Act
            double score = validator.CalculateScore(ev, null, timeslot, currentSolution, softConstraints);

            // Assert
            Assert.Equal(50, score);
        }


    }
}