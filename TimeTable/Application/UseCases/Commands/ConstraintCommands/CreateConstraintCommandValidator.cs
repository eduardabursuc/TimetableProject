using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        public CreateConstraintCommandValidator(ConstraintsValidator validator, IMapper mapper, Instance instance)
        {
            RuleFor(c => c.Type)
                .IsInEnum()
                .WithMessage("Constraint type is required.");
            
        }
    }
}