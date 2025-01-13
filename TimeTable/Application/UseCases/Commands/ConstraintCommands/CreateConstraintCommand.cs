using Application.DTOs;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public class CreateConstraintCommand : IRequest<Result<Guid>>
    {
        public required string ProfessorEmail { get; set; }
        public Guid TimetableId { get; set; }
        public required string Input { get; set; }

        public CreateConstraintCommand() { }

    }
}