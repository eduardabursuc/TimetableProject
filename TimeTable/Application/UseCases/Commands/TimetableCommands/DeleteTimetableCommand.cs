using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public record DeleteTimetableCommand(string UserEmail, Guid Id) : IRequest<Result<Unit>>;
}
