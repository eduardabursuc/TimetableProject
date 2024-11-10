using Domain.Common;
using MediatR;
using Application.DTOs;

namespace Application.UseCases.Commands
{
    public class CreateProfessorCommand : IRequest<Result<Guid>>
    {
        public required string Name { get; set; }

        public CreateProfessorCommand() { }
        
        public CreateProfessorCommand(ProfessorDto professor)
        {
            Name = professor.Name;
        }
    }
}