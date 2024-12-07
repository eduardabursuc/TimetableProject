using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.CourseCommands
{
    public record DeleteCourseCommand(Guid Id) : IRequest<Result<Unit>>;
}