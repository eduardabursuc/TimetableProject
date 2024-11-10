using Domain.Common;
using MediatR;
using Application.DTOs;
using Domain.Entities;

namespace Application.UseCases.Commands
{
    public class CreateCourseCommand : IRequest<Result<string>>
    {
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string Package { get; set; }
        public int Semester { get; set; }
        public string Level { get; set; }
        
        public CreateCourseCommand() {}

        public CreateCourseCommand(CourseDTO course)
        {
            CourseName = course.CourseName;
            Credits = course.Credits;
            Package = course.Package;
            Semester = course.Semester;
            Level = course.Level;
        }
    }
}