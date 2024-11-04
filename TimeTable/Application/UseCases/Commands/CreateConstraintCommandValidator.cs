using Application.Validators;
using Domain.Entities;
using FluentValidation;
using AutoMapper;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        private readonly IMapper mapper;

        public CreateConstraintCommandValidator(SoftConstraintsValidator validator, IMapper mapper)
        {
            this.mapper = mapper;

            RuleFor(c => c.Type)
                .IsInEnum()
                .WithMessage("Constraint type is required.");

            RuleFor(c => validator.Validate(mapper.Map<Constraint>(c)).Item1)
                .Equal(true)
                .WithMessage("Constraint is not valid.");
        }
    }
}