using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using Application.UseCases.QueryHandlers.TimetableQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

namespace TimeTable.Application.UnitTests
{
    public class GetTimetableByRoomQueryHandlerTests
    {
        private readonly ITimetableRepository _repository = Substitute.For<ITimetableRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetTimetableByRoomQueryHandler_When_HandleIsCalled_Then_TimetableShouldBeReturned()
        {
            // Arrange
            var timetable = GenerateTimetable();
            _repository.GetByRoomAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Result<Timetable>.Success(timetable));

            var query = new GetTimetableByRoomQuery { Id = timetable.Id, RoomId = timetable.Events.First().RoomId.Value };
            var timetableDto = GenerateTimetableDto(timetable);
            _mapper.Map<TimetableDto>(timetable).Returns(timetableDto);

            var handler = new GetTimetableByRoomQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(timetableDto.Id, result.Data.Id);
            Assert.Equal(timetableDto.Name, result.Data.Name);
        }

        [Fact]
        public async Task Given_GetTimetableByRoomQueryHandler_When_TimetableNotFound_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            _repository.GetByRoomAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Result<Timetable>.Failure("Timetable not found"));

            var query = new GetTimetableByRoomQuery { Id = Guid.NewGuid(), RoomId = Guid.NewGuid() };
            var handler = new GetTimetableByRoomQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Timetable not found", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_GetTimetableByRoomQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var timetable = GenerateTimetable();
            _repository.GetByRoomAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Result<Timetable>.Success(timetable));

            _mapper.Map<TimetableDto>(timetable).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetTimetableByRoomQuery { Id = timetable.Id, RoomId = timetable.Events.First().RoomId.Value };
            var handler = new GetTimetableByRoomQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        private static Timetable GenerateTimetable()
        {
            return new Timetable
            {
                UserEmail = "some1@gmail.com",
                Id = Guid.NewGuid(),
                Name = "Timetable 1",
                CreatedAt = DateTime.Now,
                Events = new List<Event>
                {
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        EventName = "Event 1",
                        CourseId = Guid.NewGuid(),
                        ProfessorId = Guid.NewGuid(),
                        GroupId = Guid.NewGuid(),
                        Duration = 2,
                        RoomId = Guid.NewGuid(),
                        Timeslot = new Timeslot { Day = "Monday", Time = "08:00-10:00" }
                    }
                }
            };
        }

        private static TimetableDto GenerateTimetableDto(Timetable timetable)
        {
            return new TimetableDto
            {
                UserEmail = timetable.UserEmail,
                Id = timetable.Id,
                Name = timetable.Name,
                CreatedAt = timetable.CreatedAt,
                Events = timetable.Events
            };
        }
    }
}