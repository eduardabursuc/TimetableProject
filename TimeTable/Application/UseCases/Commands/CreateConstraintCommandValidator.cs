using Application.Validators;
using Domain.Entities;
using FluentValidation;
using AutoMapper;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        private readonly IMapper _mapper;

        public CreateConstraintCommandValidator(SoftConstraintsValidator validator, IMapper mapper)
        {
            _mapper = mapper;

            RuleFor(c => c.Type)
                .IsInEnum()
                .WithMessage("Constraint type is required.");

            RuleFor(c => validator.Validate(_mapper.Map<Constraint>(c)).Item1)
                .Equal(false)
                .WithMessage("Constraint is not valid.");
        }
    }
}