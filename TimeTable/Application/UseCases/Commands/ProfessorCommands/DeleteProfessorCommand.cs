using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public record DeleteProfessorCommand(string UserEmail, Guid Id) : IRequest<Result<Unit>>;
}