using Application.Validators;
using Domain.Entities;
using FluentValidation;
using AutoMapper;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        public CreateConstraintCommandValidator(ConstraintsValidator validator, IMapper mapper, Instance instance)
        {
            RuleFor(c => c.Type)
                .IsInEnum()
                .WithMessage("Constraint type is required.");

            RuleFor(c => c)
                .Must(c =>
                {
                    var result = validator.Validate(mapper.Map<Constraint>(c));
                    return result.Item1;
                })
                .WithMessage(c =>
                {
                    var result = validator.Validate(mapper.Map<Constraint>(c));
                    return result.Item2;
                });
        }
    }
}

