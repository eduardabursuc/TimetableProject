using Application.Validators;
using Domain.Entities;
using Domain.Repositories;
using Domain.Common;
using NSubstitute;
using Xunit;

namespace TimeTable.Application.UnitTests
{
    public class HardConstraintValidatorTests
    {
        private readonly ICourseRepository _courseRepo = Substitute.For<ICourseRepository>();
        private readonly IGroupRepository _groupRepo = Substitute.For<IGroupRepository>();
        private readonly HardConstraintValidator _validator;

        public HardConstraintValidatorTests()
        {
            _validator = new HardConstraintValidator(_courseRepo, _groupRepo);
        }

        [Fact]
        public void ValidateNoOverlap_DifferentRooms_ReturnsTrue()
        {
            // Arrange
            var value1 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });
            var value2 = (new Room { Id = Guid.NewGuid(), Name = "Room B", Capacity = 50, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });

            // Act
            var result = _validator.ValidateNoOverlap(value1, value2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateRoomCapacity_CourseCapacityValid_ReturnsTrue()
        {
            // Arrange
            var room = new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" };
            var eventName = "course";

            // Act
            var result = _validator.ValidateRoomCapacity(room, eventName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateGroupOverlap_GroupsOverlap_ReturnsFalse()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), EventName = "Event 1", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var event2 = new Event { Id = Guid.NewGuid(), EventName = "Event 2", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var value1 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });
            var value2 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" });

            var group = new Group { Id = Guid.NewGuid(), Name = "2E", UserEmail = "test@example.com" };
            var course1 = new Course { Id = Guid.NewGuid(), CourseName = "Course 1", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };
            var course2 = new Course { Id = Guid.NewGuid(), CourseName = "Course 2", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };

            _groupRepo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(Result<Group>.Success(group)));
            _courseRepo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(Result<Course>.Success(course1)));

            // Act
            var result = _validator.ValidateGroupOverlap(event1, event2, value1, value2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TimeslotsOverlap_Overlap_ReturnsTrue()
        {
            // Arrange
            var timeslot1 = new Timeslot { Day = "Monday", Time = "09:00 - 11:00" };
            var timeslot2 = new Timeslot { Day = "Monday", Time = "10:00 - 12:00" };
            var duration1 = 2;
            var duration2 = 2;

            // Act
            var result = _validator.TimeslotsOverlap(timeslot1, duration1, timeslot2, duration2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameOrNestedGroup_SameGroup_ReturnsTrue()
        {
            // Arrange
            var group1 = "2E";
            var group2 = "2E3";

            // Act
            var result = HardConstraintValidator.IsSameOrNestedGroup(group1, group2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateRoomCapacity_SeminaryCapacityValid_ReturnsTrue()
        {
            // Arrange
            var room = new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 30, UserEmail = "test@example.com" };
            var eventName = "seminary";

            // Act
            var result = _validator.ValidateRoomCapacity(room, eventName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateRoomCapacity_LaboratoryCapacityInvalid_ReturnsFalse()
        {
            // Arrange
            var room = new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 20, UserEmail = "test@example.com" };
            var eventName = "laboratory";

            // Act
            var result = _validator.ValidateRoomCapacity(room, eventName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateGroupOverlap_DifferentGroups_ReturnsTrue()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), EventName = "Event 1", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var event2 = new Event { Id = Guid.NewGuid(), EventName = "Event 2", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var value1 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });
            var value2 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "12:00 - 14:00" });

            var group1 = new Group { Id = Guid.NewGuid(), Name = "2E", UserEmail = "test@example.com" };
            var group2 = new Group { Id = Guid.NewGuid(), Name = "3E", UserEmail = "test@example.com" };
            var course1 = new Course { Id = Guid.NewGuid(), CourseName = "Course 1", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };
            var course2 = new Course { Id = Guid.NewGuid(), CourseName = "Course 2", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };

            _groupRepo.GetByIdAsync(event1.GroupId).Returns(Task.FromResult(Result<Group>.Success(group1)));
            _groupRepo.GetByIdAsync(event2.GroupId).Returns(Task.FromResult(Result<Group>.Success(group2)));
            _courseRepo.GetByIdAsync(event1.CourseId).Returns(Task.FromResult(Result<Course>.Success(course1)));
            _courseRepo.GetByIdAsync(event2.CourseId).Returns(Task.FromResult(Result<Course>.Success(course2)));

            // Act
            var result = _validator.ValidateGroupOverlap(event1, event2, value1, value2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TimeslotsOverlap_NoOverlap_ReturnsFalse()
        {
            // Arrange
            var timeslot1 = new Timeslot { Day = "Monday", Time = "09:00 - 11:00" };
            var timeslot2 = new Timeslot { Day = "Monday", Time = "12:00 - 14:00" };
            var duration1 = 2;
            var duration2 = 2;

            // Act
            var result = _validator.TimeslotsOverlap(timeslot1, duration1, timeslot2, duration2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameOrNestedGroup_DifferentGroup_ReturnsFalse()
        {
            // Arrange
            var group1 = "2E";
            var group2 = "3E";

            // Act
            var result = HardConstraintValidator.IsSameOrNestedGroup(group1, group2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateGroupOverlap_SameCourse_Overlap_ReturnsFalse()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), EventName = "Event 1", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var event2 = new Event { Id = Guid.NewGuid(), EventName = "Event 2", CourseId = event1.CourseId, ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var value1 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });
            var value2 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" });

            var group = new Group { Id = Guid.NewGuid(), Name = "2E", UserEmail = "test@example.com" };
            var course = new Course { Id = event1.CourseId, CourseName = "Course 1", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };

            _groupRepo.GetByIdAsync(event1.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _groupRepo.GetByIdAsync(event2.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _courseRepo.GetByIdAsync(event1.CourseId).Returns(Task.FromResult(Result<Course>.Success(course)));

            // Act
            var result = _validator.ValidateGroupOverlap(event1, event2, value1, value2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateGroupOverlap_CompulsoryCourse_Overlap_ReturnsFalse()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), EventName = "Event 1", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var event2 = new Event { Id = Guid.NewGuid(), EventName = "Event 2", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var value1 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });
            var value2 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" });

            var group = new Group { Id = Guid.NewGuid(), Name = "2E", UserEmail = "test@example.com" };
            var course1 = new Course { Id = Guid.NewGuid(), CourseName = "Course 1", Credits = 3, Package = "compulsory", Semester = 1, Level = "1", UserEmail = "test@example.com" };
            var course2 = new Course { Id = Guid.NewGuid(), CourseName = "Course 2", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };

            _groupRepo.GetByIdAsync(event1.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _groupRepo.GetByIdAsync(event2.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _courseRepo.GetByIdAsync(event1.CourseId).Returns(Task.FromResult(Result<Course>.Success(course1)));
            _courseRepo.GetByIdAsync(event2.CourseId).Returns(Task.FromResult(Result<Course>.Success(course2)));

            // Act
            var result = _validator.ValidateGroupOverlap(event1, event2, value1, value2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateGroupOverlap_SamePackageSameLevelSameSemester_Overlap_ReturnsFalse()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), EventName = "Event 1", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var event2 = new Event { Id = Guid.NewGuid(), EventName = "Event 2", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var value1 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });
            var value2 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" });

            var group = new Group { Id = Guid.NewGuid(), Name = "2E", UserEmail = "test@example.com" };
            var course1 = new Course { Id = Guid.NewGuid(), CourseName = "Course 1", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };
            var course2 = new Course { Id = Guid.NewGuid(), CourseName = "Course 2", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };

            _groupRepo.GetByIdAsync(event1.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _groupRepo.GetByIdAsync(event2.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _courseRepo.GetByIdAsync(event1.CourseId).Returns(Task.FromResult(Result<Course>.Success(course1)));
            _courseRepo.GetByIdAsync(event2.CourseId).Returns(Task.FromResult(Result<Course>.Success(course2)));

            // Act
            var result = _validator.ValidateGroupOverlap(event1, event2, value1, value2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateRoomCapacity_InvalidEventName_ReturnsTrue()
        {
            // Arrange
            var room = new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 30, UserEmail = "test@example.com" };
            var eventName = "workshop";

            // Act
            var result = _validator.ValidateRoomCapacity(room, eventName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateGroupOverlap_CourseRepoFailure_ReturnsTrue()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), EventName = "Event 1", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var event2 = new Event { Id = Guid.NewGuid(), EventName = "Event 2", CourseId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(), GroupId = Guid.NewGuid(), Duration = 2 };
            var value1 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "09:00 - 11:00" });
            var value2 = (new Room { Id = Guid.NewGuid(), Name = "Room A", Capacity = 100, UserEmail = "test@example.com" }, new Timeslot { Day = "Monday", Time = "10:00 - 12:00" });

            var group = new Group { Id = Guid.NewGuid(), Name = "2E", UserEmail = "test@example.com" };
            var course1 = new Course { Id = Guid.NewGuid(), CourseName = "Course 1", Credits = 3, Package = "optional", Semester = 1, Level = "1", UserEmail = "test@example.com" };

            _groupRepo.GetByIdAsync(event1.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _groupRepo.GetByIdAsync(event2.GroupId).Returns(Task.FromResult(Result<Group>.Success(group)));
            _courseRepo.GetByIdAsync(event1.CourseId).Returns(Task.FromResult(Result<Course>.Failure("Failed to fetch course")));

            // Act
            var result = _validator.ValidateGroupOverlap(event1, event2, value1, value2);

            // Assert
            Assert.True(result);
        }
    }
}
