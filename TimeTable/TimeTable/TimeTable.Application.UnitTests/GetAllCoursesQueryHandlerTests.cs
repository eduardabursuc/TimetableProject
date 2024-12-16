using Application.DTOs;
using Application.UseCases.Queries.CourseQueries;
using Application.UseCases.QueryHandlers.CourseQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace TimeTable.Application.UnitTests
{
    public class GetAllCoursesQueryHandlerTests
    {
        private readonly ICourseRepository _repository = Substitute.For<ICourseRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetAllCoursesQueryHandler_When_HandleIsCalled_Then_AListOfCoursesShouldBeReturned()
        {
            // Arrange
            var courses = GenerateCourseList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Course>>.Success(courses));

            var query = new GetAllCoursesQuery { UserEmail = "some1@gmail.com" };
            var courseDtos = GenerateCourseDto(courses.ToList());
            _mapper.Map<List<CourseDto>>(courses).Returns(courseDtos);

            var handler = new GetAllCoursesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(courseDtos.Count, result.Data.Count);
            Assert.Equal(courseDtos[0], result.Data[0]);
            Assert.Equal(courseDtos[1], result.Data[1]);
        }

        [Fact]
        public async Task Given_GetAllCoursesQueryHandler_When_NoCoursesInRepository_Then_EmptyListShouldBeReturned()
        {
            // Arrange
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Course>>.Success(new List<Course>()));

            var query = new GetAllCoursesQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllCoursesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Given_GetAllCoursesQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var courses = GenerateCourseList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Course>>.Success(courses));

            _mapper.Map<List<CourseDto>>(courses).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetAllCoursesQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllCoursesQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllCoursesQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var courses = GenerateCourseList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Course>>.Success(courses));

            var courseDtos = GenerateCourseDto(courses.ToList());
            _mapper.Map<List<CourseDto>>(courses).Returns(courseDtos);

            var query = new GetAllCoursesQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllCoursesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(courseDtos[0].Id, result.Data[0].Id);
            Assert.Equal(courseDtos[0].CourseName, result.Data[0].CourseName);
            Assert.Equal(courseDtos[0].Credits, result.Data[0].Credits);
            Assert.Equal(courseDtos[0].Package, result.Data[0].Package);
            Assert.Equal(courseDtos[0].Semester, result.Data[0].Semester);
            Assert.Equal(courseDtos[0].Level, result.Data[0].Level);
        }

        private static List<Course> GenerateCourseList()
        {
            return new List<Course>
            {
                new Course
                {
                    UserEmail = "some1@gmail.com",
                    Id = Guid.NewGuid(),
                    CourseName = "Course 1",
                    Credits = 3,
                    Package = "Package 1",
                    Semester = 1,
                    Level = "Undergraduate"
                },
                new Course
                {
                    UserEmail = "some2@gmail.com",
                    Id = Guid.NewGuid(),
                    CourseName = "Course 2",
                    Credits = 4,
                    Package = "Package 2",
                    Semester = 2,
                    Level = "Graduate"
                }
            };
        }

        private static List<CourseDto> GenerateCourseDto(List<Course> courses)
        {
            return courses.Select(c => new CourseDto
            {
                UserEmail = c.UserEmail,
                Id = c.Id,
                CourseName = c.CourseName,
                Credits = c.Credits,
                Package = c.Package,
                Semester = c.Semester,
                Level = c.Level
            }).ToList();
        }
    }
}