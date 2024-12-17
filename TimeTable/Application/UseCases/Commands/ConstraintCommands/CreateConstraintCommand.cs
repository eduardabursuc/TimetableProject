using Application.DTOs;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public class CreateConstraintCommand : IRequest<Result<Guid>>
    {
        public string ProfessorEmail { get; set; }
        public Guid TimetableId { get; set; }
        public string Input { get; set; }

        public CreateConstraintCommand() { }

        public CreateConstraintCommand(string professorEmail, Guid timetableId, string input)
        {
            ProfessorEmail = professorEmail;
            TimetableId = timetableId;
            Input = input;
        }

    }
}