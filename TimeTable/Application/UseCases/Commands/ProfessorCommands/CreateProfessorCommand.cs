using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public class CreateProfessorCommand : IRequest<Result<Guid>>
    {
        public required string Name { get; init; }

        protected CreateProfessorCommand() { }
        
        public CreateProfessorCommand(ProfessorDto professor)
        {
            Name = professor.Name;
        }
    }
}