using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.GroupCommands
{
    public record DeleteGroupCommand(Guid Id) : IRequest<Result<Unit>>;
}


