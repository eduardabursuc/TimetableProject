using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands
{
    public record DeleteCourseCommand(string CourseName) : IRequest<Result<Unit>>;
}