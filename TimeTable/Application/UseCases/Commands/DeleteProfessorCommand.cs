using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands
{
    public record DeleteProfessorCommand(Guid Id) : IRequest<Result<Unit>>;
}