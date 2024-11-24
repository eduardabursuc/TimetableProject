using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.CourseCommands
{
    public record DeleteCourseCommand(string CourseName) : IRequest<Result<Unit>>;
}