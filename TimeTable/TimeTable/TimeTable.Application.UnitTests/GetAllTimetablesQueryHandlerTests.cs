using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using Application.UseCases.QueryHandlers.TimetableQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace TimeTable.Application.UnitTests
{
    public class GetAllTimetablesQueryHandlerTests
    {
        private readonly ITimetableRepository _repository = Substitute.For<ITimetableRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetAllTimetablesQueryHandler_When_HandleIsCalled_Then_AListOfTimetablesShouldBeReturned()
        {
            // Arrange
            var timetables = GenerateTimetableList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Timetable>>.Success(timetables));

            var query = new GetAllTimetablesQuery { UserEmail = "some1@gmail.com" };
            var timetableDtos = GenerateTimetableDto(timetables.ToList());
            _mapper.Map<List<TimetableDto>>(timetables).Returns(timetableDtos);

            var handler = new GetAllTimetablesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(timetableDtos.Count, result.Data.Count);
            Assert.Equal(timetableDtos[0], result.Data[0]);
            Assert.Equal(timetableDtos[1], result.Data[1]);
        }

        [Fact]
        public async Task Given_GetAllTimetablesQueryHandler_When_NoTimetablesInRepository_Then_EmptyListShouldBeReturned()
        {
            // Arrange
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Timetable>>.Success(new List<Timetable>()));

            var query = new GetAllTimetablesQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllTimetablesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Given_GetAllTimetablesQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var timetables = GenerateTimetableList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Timetable>>.Success(timetables));

            _mapper.Map<List<TimetableDto>>(timetables).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetAllTimetablesQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllTimetablesQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllTimetablesQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var timetables = GenerateTimetableList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Timetable>>.Success(timetables));

            var timetableDtos = GenerateTimetableDto(timetables.ToList());
            _mapper.Map<List<TimetableDto>>(timetables).Returns(timetableDtos);

            var query = new GetAllTimetablesQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllTimetablesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(timetableDtos[0].Id, result.Data[0].Id);
            Assert.Equal(timetableDtos[0].Name, result.Data[0].Name);
            Assert.Equal(timetableDtos[0].CreatedAt, result.Data[0].CreatedAt);
        }

        private static List<Timetable> GenerateTimetableList()
        {
            return new List<Timetable>
            {
                new Timetable
                {
                    UserEmail = "some1@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Timetable 1",
                    CreatedAt = DateTime.Now
                },
                new Timetable
                {
                    UserEmail = "some2@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Timetable 2",
                    CreatedAt = DateTime.Now
                }
            };
        }

        private static List<TimetableDto> GenerateTimetableDto(List<Timetable> timetables)
        {
            return timetables.Select(t => new TimetableDto
            {
                UserEmail = t.UserEmail,
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
    }
}