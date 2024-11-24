using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.CourseCommands
{
    public class CreateCourseCommand : IRequest<Result<string>>
    {
        public required string CourseName { get; init; }
        public required int Credits { get; init; }
        public required string Package { get; init; }
        public required int Semester { get; init; }
        public required string Level { get; init; }

        protected CreateCourseCommand() { }

        public CreateCourseCommand(CourseDto course)
        {
            CourseName = course.CourseName;
            Credits = course.Credits;
            Package = course.Package;
            Semester = course.Semester;
            Level = course.Level;
        }
    }
}