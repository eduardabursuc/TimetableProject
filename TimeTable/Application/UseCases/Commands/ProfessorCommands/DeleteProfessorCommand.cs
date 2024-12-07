using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public record DeleteProfessorCommand(Guid Id) : IRequest<Result<Unit>>;
}