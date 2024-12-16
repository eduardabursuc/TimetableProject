using Application.DTOs;
using Application.UseCases.Queries.ConstraintQueries;
using Application.UseCases.QueryHandlers.ConstraintQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

namespace TimeTable.Application.UnitTests
{
    public class GetAllConstraintsQueryHandlerTests
    {
        private readonly IConstraintRepository _repository = Substitute.For<IConstraintRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetAllConstraintsQueryHandler_When_HandleIsCalled_Then_AListOfConstraintsShouldBeReturned()
        {
            // Arrange
            var constraints = GenerateConstraintList();
            _repository.GetAllAsync(Arg.Any<Guid>()).Returns(Result<IEnumerable<Constraint>>.Success(constraints));

            var query = new GetAllConstraintsQuery { TimetableId = Guid.NewGuid() };
            var constraintDtos = GenerateConstraintDto(constraints.ToList());
            _mapper.Map<List<ConstraintDto>>(constraints).Returns(constraintDtos);

            var handler = new GetAllConstraintsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(constraintDtos.Count, result.Data.Count);
            Assert.Equal(constraintDtos[0], result.Data[0]);
            Assert.Equal(constraintDtos[1], result.Data[1]);
        }

        [Fact]
        public async Task Given_GetAllConstraintsQueryHandler_When_NoConstraintsInRepository_Then_EmptyListShouldBeReturned()
        {
            // Arrange
            _repository.GetAllAsync(Arg.Any<Guid>()).Returns(Result<IEnumerable<Constraint>>.Success(new List<Constraint>()));

            var query = new GetAllConstraintsQuery { TimetableId = Guid.NewGuid() };
            var handler = new GetAllConstraintsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Given_GetAllConstraintsQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var constraints = GenerateConstraintList();
            _repository.GetAllAsync(Arg.Any<Guid>()).Returns(Result<IEnumerable<Constraint>>.Success(constraints));

            _mapper.Map<List<ConstraintDto>>(constraints).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetAllConstraintsQuery { TimetableId = Guid.NewGuid() };
            var handler = new GetAllConstraintsQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllConstraintsQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var constraints = GenerateConstraintList();
            _repository.GetAllAsync(Arg.Any<Guid>()).Returns(Result<IEnumerable<Constraint>>.Success(constraints));

            var constraintDtos = GenerateConstraintDto(constraints.ToList());
            _mapper.Map<List<ConstraintDto>>(constraints).Returns(constraintDtos);

            var query = new GetAllConstraintsQuery { TimetableId = Guid.NewGuid() };
            var handler = new GetAllConstraintsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(constraintDtos[0].Id, result.Data[0].Id);
            Assert.Equal(constraintDtos[0].Type, result.Data[0].Type);
            Assert.Equal(constraintDtos[0].ProfessorId, result.Data[0].ProfessorId);
            Assert.Equal(constraintDtos[0].CourseName, result.Data[0].CourseName);
            Assert.Equal(constraintDtos[0].RoomName, result.Data[0].RoomName);
            Assert.Equal(constraintDtos[0].WantedRoomName, result.Data[0].WantedRoomName);
            Assert.Equal(constraintDtos[0].GroupName, result.Data[0].GroupName);
            Assert.Equal(constraintDtos[0].Day, result.Data[0].Day);
            Assert.Equal(constraintDtos[0].Time, result.Data[0].Time);
            Assert.Equal(constraintDtos[0].WantedDay, result.Data[0].WantedDay);
            Assert.Equal(constraintDtos[0].WantedTime, result.Data[0].WantedTime);
            Assert.Equal(constraintDtos[0].Event, result.Data[0].Event);
        }

        private static List<Constraint> GenerateConstraintList()
        {
            return new List<Constraint>
            {
                new Constraint
                {
                    Id = Guid.NewGuid(),
                    Type = ConstraintType.HARD_NO_OVERLAP,
                    ProfessorId = Guid.NewGuid(),
                    CourseId = Guid.NewGuid(),
                    RoomId = Guid.NewGuid(),
                    WantedRoomId = Guid.NewGuid(),
                    GroupId = Guid.NewGuid(),
                    Day = "Monday",
                    Time = "10:00",
                    WantedDay = "Tuesday",
                    WantedTime = "11:00",
                    Event = "Event 1"
                },
                new Constraint
                {
                    Id = Guid.NewGuid(),
                    Type = ConstraintType.SOFT_ROOM_CHANGE,
                    ProfessorId = Guid.NewGuid(),
                    CourseId = Guid.NewGuid(),
                    RoomId = Guid.NewGuid(),
                    WantedRoomId = Guid.NewGuid(),
                    GroupId = Guid.NewGuid(),
                    Day = "Wednesday",
                    Time = "12:00",
                    WantedDay = "Thursday",
                    WantedTime = "13:00",
                    Event = "Event 2"
                }
            };
        }

        private static List<ConstraintDto> GenerateConstraintDto(List<Constraint> constraints)
        {
            return constraints.Select(c => new ConstraintDto
            {
                Id = c.Id,
                Type = c.Type,
                ProfessorId = c.ProfessorId,
                CourseName = c.CourseId.ToString(), // Assuming CourseName is derived from CourseId
                RoomName = c.RoomId.ToString(), // Assuming RoomName is derived from RoomId
                WantedRoomName = c.WantedRoomId.ToString(), // Assuming WantedRoomName is derived from WantedRoomId
                GroupName = c.GroupId.ToString(), // Assuming GroupName is derived from GroupId
                Day = c.Day,
                Time = c.Time,
                WantedDay = c.WantedDay,
                WantedTime = c.WantedTime,
                Event = c.Event
            }).ToList();
        }
    }
}