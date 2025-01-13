using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        public CreateConstraintCommandValidator()
        {
            RuleFor(c => c.ProfessorEmail)
                .NotEmpty()
                .WithMessage("Professor email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(c => c.TimetableId)
                .NotEmpty()
                .WithMessage("Timetable ID is required.");

            RuleFor(c => c.Input)
                .NotEmpty()
                .WithMessage("Input is required.");
        }
    }
}