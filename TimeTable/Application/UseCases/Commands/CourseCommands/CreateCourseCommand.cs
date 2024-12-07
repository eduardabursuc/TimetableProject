using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.CourseCommands
{
    public class CreateCourseCommand : IRequest<Result<Guid>>
    {
        public required string UserEmail { get; init; }
        public required string CourseName { get; init; }
        public required int Credits { get; init; }
        public required string Package { get; init; }
        public required int Semester { get; init; }
        public required string Level { get; init; }

        public CreateCourseCommand() { }
        
    }
}