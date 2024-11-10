using Domain.Common;
using MediatR;
using Application.DTOs;
using Domain.Entities;

namespace Application.UseCases.Commands
{
    public class CreateCourseCommand : IRequest<Result<string>>
    {
        public required string CourseName { get; set; }
        public required int Credits { get; set; }
        public required string Package { get; set; }
        public required int Semester { get; set; }
        public required string Level { get; set; }

        public CreateCourseCommand() { }

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