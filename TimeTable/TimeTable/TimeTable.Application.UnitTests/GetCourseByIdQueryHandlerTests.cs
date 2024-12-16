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
    public class GetCourseByIdQueryHandlerTests
    {
        private readonly ICourseRepository _repository = Substitute.For<ICourseRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetCourseByIdQueryHandler_When_HandleIsCalled_Then_CourseShouldBeReturned()
        {
            // Arrange
            var course = GenerateCourse();
            _repository.GetByIdAsync(course.Id).Returns(Result<Course>.Success(course));

            var query = new GetCourseByIdQuery { CourseId = course.Id };
            var courseDto = GenerateCourseDto(course);
            _mapper.Map<CourseDto>(course).Returns(courseDto);

            var handler = new GetCourseByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(courseDto, result.Data);
        }

        [Fact]
        public async Task Given_GetCourseByIdQueryHandler_When_CourseNotFound_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _repository.GetByIdAsync(courseId).Returns(Result<Course>.Failure("Course not found"));

            var query = new GetCourseByIdQuery { CourseId = courseId };
            var handler = new GetCourseByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Course not found", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_GetCourseByIdQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var course = GenerateCourse();
            _repository.GetByIdAsync(course.Id).Returns(Result<Course>.Success(course));

            _mapper.Map<CourseDto>(course).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetCourseByIdQuery { CourseId = course.Id };
            var handler = new GetCourseByIdQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetCourseByIdQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var course = GenerateCourse();
            _repository.GetByIdAsync(course.Id).Returns(Result<Course>.Success(course));

            var courseDto = GenerateCourseDto(course);
            _mapper.Map<CourseDto>(course).Returns(courseDto);

            var query = new GetCourseByIdQuery { CourseId = course.Id };
            var handler = new GetCourseByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(courseDto.Id, result.Data.Id);
            Assert.Equal(courseDto.CourseName, result.Data.CourseName);
            Assert.Equal(courseDto.Credits, result.Data.Credits);
            Assert.Equal(courseDto.Package, result.Data.Package);
            Assert.Equal(courseDto.Semester, result.Data.Semester);
            Assert.Equal(courseDto.Level, result.Data.Level);
        }

        private static Course GenerateCourse()
        {
            return new Course
            {
                UserEmail = "some1@gmail.com",
                Id = Guid.NewGuid(),
                CourseName = "Course 1",
                Credits = 3,
                Package = "Package 1",
                Semester = 1,
                Level = "Undergraduate"
            };
        }

        private static CourseDto GenerateCourseDto(Course course)
        {
            return new CourseDto
            {
                UserEmail = course.UserEmail,
                Id = course.Id,
                CourseName = course.CourseName,
                Credits = course.Credits,
                Package = course.Package,
                Semester = course.Semester,
                Level = course.Level
            };
        }
    }
}