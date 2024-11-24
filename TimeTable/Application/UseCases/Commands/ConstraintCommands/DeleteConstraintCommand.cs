using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public record DeleteConstraintCommand(Guid Id) : IRequest<Result<Unit>>;
}