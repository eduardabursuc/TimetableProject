using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public class CreateProfessorCommand : IRequest<Result<Guid>>
    {
        public required string UserEmail { get; init; }
        public required string Name { get; init; }

        public CreateProfessorCommand() { }
        
    }
}