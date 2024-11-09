using Domain.Common;
using MediatR;
using Application.DTOs;

namespace Application.UseCases.Commands
{
    public class CreateProfessorCommand : IRequest<Result<Guid>>
    {
        public string Name { get; set; }

        public CreateProfessorCommand() { }
        
        public CreateProfessorCommand(ProfessorDTO professor)
        {
            Name = professor.Name;
        }
    }
}