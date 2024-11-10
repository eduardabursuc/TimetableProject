using Application.DTOs;
using Application.UseCases.Queries;
using Application.UseCases.QueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

namespace Application.UnitTests
{
    public class GetAllConstraintsQueryHandlerTests
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public GetAllConstraintsQueryHandlerTests()
        {
            repository = Substitute.For<IConstraintRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetAllConstraintsQueryHandler_When_HandleIsCalled_Then_AListOfConstraintsShouldBeReturned()
        {
            // Arrange
            IEnumerable<Constraint> constraints = GenerateConstraintList();
            repository.GetAllAsync().Returns(Result<IEnumerable<Constraint>>.Success(constraints));

            var query = new GetAllConstraintsQuery();
            var constraintDtos = GenerateConstraintDto(constraints.ToList());
            mapper.Map<List<ConstraintDto>>(constraints).Returns(constraintDtos);

            var handler = new GetAllConstraintsQueryHandler(repository, mapper);

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
            repository.GetAllAsync().Returns(Result<IEnumerable<Constraint>>.Success(new List<Constraint>()));

            var query = new GetAllConstraintsQuery();
            var handler = new GetAllConstraintsQueryHandler(repository, mapper);

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
            IEnumerable<Constraint> constraints = GenerateConstraintList();
            repository.GetAllAsync().Returns(Result<IEnumerable<Constraint>>.Success(constraints));

            mapper.Map<List<ConstraintDto>>(constraints).Returns(x => { throw new Exception("Mapping failed"); });

            var query = new GetAllConstraintsQuery();
            var handler = new GetAllConstraintsQueryHandler(repository, mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllConstraintsQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            IEnumerable<Constraint> constraints = GenerateConstraintList();
            repository.GetAllAsync().Returns(Result<IEnumerable<Constraint>>.Success(constraints));

            var constraintDtos = GenerateConstraintDto(constraints.ToList());
            mapper.Map<List<ConstraintDto>>(constraints).Returns(constraintDtos);

            var query = new GetAllConstraintsQuery();
            var handler = new GetAllConstraintsQueryHandler(repository, mapper);

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
                    CourseName = "Course 1",
                    RoomName = "Room 1",
                    WantedRoomName = "Room 2",
                    GroupName = "Group 1",
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
                    CourseName = "Course 2",
                    RoomName = "Room 3",
                    WantedRoomName = "Room 4",
                    GroupName = "Group 2",
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
            return new List<ConstraintDto>
            {
                new ConstraintDto
                {
                    Id = constraints[0].Id,
                    Type = constraints[0].Type,
                    ProfessorId = constraints[0].ProfessorId,
                    CourseName = constraints[0].CourseName,
                    RoomName = constraints[0].RoomName,
                    WantedRoomName = constraints[0].WantedRoomName,
                    GroupName = constraints[0].GroupName,
                    Day = constraints[0].Day,
                    Time = constraints[0].Time,
                    WantedDay = constraints[0].WantedDay,
                    WantedTime = constraints[0].WantedTime,
                    Event = constraints[0].Event
                },
                new ConstraintDto
                {
                    Id = constraints[1].Id,
                    Type = constraints[1].Type,
                    ProfessorId = constraints[1].ProfessorId,
                    CourseName = constraints[1].CourseName,
                    RoomName = constraints[1].RoomName,
                    WantedRoomName = constraints[1].WantedRoomName,
                    GroupName = constraints[1].GroupName,
                    Day = constraints[1].Day,
                    Time = constraints[1].Time,
                    WantedDay = constraints[1].WantedDay,
                    WantedTime = constraints[1].WantedTime,
                    Event = constraints[1].Event
                }
            };
        }
    }
}