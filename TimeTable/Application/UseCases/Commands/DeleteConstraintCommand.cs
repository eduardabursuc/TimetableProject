using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands
{
    public record DeleteConstraintCommand(Guid Id) : IRequest<Result<Unit>>;
}