using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public abstract class UpdateConstraintCommandValidator : AbstractValidator<UpdateConstraintCommand>
    {
        protected UpdateConstraintCommandValidator(ConstraintsValidator validator, IMapper mapper, Instance instance)
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateConstraintCommandValidator(validator, mapper, instance));
        }
    }
}