using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public record DeleteTimetableCommand(Guid Id) : IRequest<Result<Unit>>;
}
