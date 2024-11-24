using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public class UpdateProfessorCommand : CreateProfessorCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}