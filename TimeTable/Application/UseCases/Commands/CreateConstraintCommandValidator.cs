using Application.Validators;
using Domain.Entities;
using FluentValidation;
using AutoMapper;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        private readonly IMapper mapper;

        public CreateConstraintCommandValidator(ConstraintsValidator validator, IMapper mapper)
        {
            this.mapper = mapper;

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