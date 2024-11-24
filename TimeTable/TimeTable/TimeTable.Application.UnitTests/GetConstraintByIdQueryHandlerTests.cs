using Application.DTOs;
using Application.UseCases.Queries;
using Application.UseCases.Queries.ConstraintQueries;
using Application.UseCases.QueryHandlers;
using Application.UseCases.QueryHandlers.ConstraintQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

namespace TimeTable.Application.UnitTests
{
    public class GetConstraintByIdQueryHandlerTests
    {
        private readonly IConstraintRepository _repository = Substitute.For<IConstraintRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetConstraintByIdQueryHandler_When_HandleIsCalled_Then_ConstraintShouldBeReturned()
        {
            // Arrange
            var constraint = GenerateConstraint();
            _repository.GetByIdAsync(constraint.Id).Returns(Result<Constraint>.Success(constraint));

            var query = new GetConstraintByIdQuery { Id = constraint.Id };
            var constraintDto = GenerateConstraintDto(constraint);
            _mapper.Map<ConstraintDto>(constraint).Returns(constraintDto);

            var handler = new GetConstraintByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(constraintDto, result.Data);
        }

        [Fact]
        public async Task Given_GetConstraintByIdQueryHandler_When_ConstraintNotFound_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var constraintId = Guid.NewGuid();
            _repository.GetByIdAsync(constraintId).Returns(Result<Constraint>.Failure("Constraint not found"));

            var query = new GetConstraintByIdQuery { Id = constraintId };
            var handler = new GetConstraintByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Constraint not found", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_GetConstraintByIdQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var constraint = GenerateConstraint();
            _repository.GetByIdAsync(constraint.Id).Returns(Result<Constraint>.Success(constraint));

            _mapper.Map<ConstraintDto>(constraint).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetConstraintByIdQuery { Id = constraint.Id };
            var handler = new GetConstraintByIdQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetConstraintByIdQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var constraint = GenerateConstraint();
            _repository.GetByIdAsync(constraint.Id).Returns(Result<Constraint>.Success(constraint));

            var constraintDto = GenerateConstraintDto(constraint);
            _mapper.Map<ConstraintDto>(constraint).Returns(constraintDto);

            var query = new GetConstraintByIdQuery { Id = constraint.Id };
            var handler = new GetConstraintByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(constraintDto.Id, result.Data.Id);
            Assert.Equal(constraintDto.Type, result.Data.Type);
            Assert.Equal(constraintDto.ProfessorId, result.Data.ProfessorId);
            Assert.Equal(constraintDto.CourseName, result.Data.CourseName);
            Assert.Equal(constraintDto.RoomName, result.Data.RoomName);
            Assert.Equal(constraintDto.WantedRoomName, result.Data.WantedRoomName);
            Assert.Equal(constraintDto.GroupName, result.Data.GroupName);
            Assert.Equal(constraintDto.Day, result.Data.Day);
            Assert.Equal(constraintDto.Time, result.Data.Time);
            Assert.Equal(constraintDto.WantedDay, result.Data.WantedDay);
            Assert.Equal(constraintDto.WantedTime, result.Data.WantedTime);
            Assert.Equal(constraintDto.Event, result.Data.Event);
        }

        private static Constraint GenerateConstraint()
        {
            return new Constraint
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
            };
        }

        private static ConstraintDto GenerateConstraintDto(Constraint constraint)
        {
            return new ConstraintDto
            {
                Id = constraint.Id,
                Type = constraint.Type,
                ProfessorId = constraint.ProfessorId,
                CourseName = constraint.CourseName,
                RoomName = constraint.RoomName,
                WantedRoomName = constraint.WantedRoomName,
                GroupName = constraint.GroupName,
                Day = constraint.Day,
                Time = constraint.Time,
                WantedDay = constraint.WantedDay,
                WantedTime = constraint.WantedTime,
                Event = constraint.Event
            };
        }
    }
}