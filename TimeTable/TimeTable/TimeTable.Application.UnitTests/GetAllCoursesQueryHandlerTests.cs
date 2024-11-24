using Application.DTOs;
using Application.UseCases.Queries;
using Application.UseCases.Queries.CourseQueries;
using Application.UseCases.QueryHandlers;
using Application.UseCases.QueryHandlers.CourseQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

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
            IEnumerable<Course> courses = GenerateCourseList();
            _repository.GetAllAsync().Returns(Result<IEnumerable<Course>>.Success(courses));

            var query = new GetAllCoursesQuery();
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
            _repository.GetAllAsync().Returns(Result<IEnumerable<Course>>.Success(new List<Course>()));

            var query = new GetAllCoursesQuery();
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
            IEnumerable<Course> courses = GenerateCourseList();
            _repository.GetAllAsync().Returns(Result<IEnumerable<Course>>.Success(courses));

            _mapper.Map<List<CourseDto>>(courses).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetAllCoursesQuery();
            var handler = new GetAllCoursesQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllCoursesQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            IEnumerable<Course> courses = GenerateCourseList();
            _repository.GetAllAsync().Returns(Result<IEnumerable<Course>>.Success(courses));

            var courseDtos = GenerateCourseDto(courses.ToList());
            _mapper.Map<List<CourseDto>>(courses).Returns(courseDtos);

            var query = new GetAllCoursesQuery();
            var handler = new GetAllCoursesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(courseDtos[0].CourseName, result.Data[0].CourseName);
            Assert.Equal(courseDtos[0].Credits, result.Data[0].Credits);
            Assert.Equal(courseDtos[0].Package, result.Data[0].Package);
            Assert.Equal(courseDtos[0].Semester, result.Data[0].Semester);
            Assert.Equal(courseDtos[0].Level, result.Data[0].Level);
        }

        private static List<Course> GenerateCourseList()
        {
            return
            [
                new Course
                {
                    CourseName = "Course 1",
                    Credits = 3,
                    Package = "Package 1",
                    Semester = 1,
                    Level = "Undergraduate"
                },

                new Course
                {
                    CourseName = "Course 2",
                    Credits = 4,
                    Package = "Package 2",
                    Semester = 2,
                    Level = "Graduate"
                }
            ];
        }

        private static List<CourseDto> GenerateCourseDto(List<Course> courses)
        {
            return
            [
                new CourseDto
                {
                    CourseName = courses[0].CourseName,
                    Credits = courses[0].Credits,
                    Package = courses[0].Package,
                    Semester = courses[0].Semester,
                    Level = courses[0].Level
                },

                new CourseDto
                {
                    CourseName = courses[1].CourseName,
                    Credits = courses[1].Credits,
                    Package = courses[1].Package,
                    Semester = courses[1].Semester,
                    Level = courses[1].Level
                }
            ];
        }
    }
}