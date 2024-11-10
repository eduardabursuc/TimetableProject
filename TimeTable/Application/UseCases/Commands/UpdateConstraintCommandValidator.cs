using FluentValidation;
using AutoMapper;
using Application.Validators;
using Domain.Entities;

namespace Application.UseCases.Commands
{
    public class UpdateConstraintCommandValidator : AbstractValidator<UpdateConstraintCommand>
    {
        public UpdateConstraintCommandValidator(ConstraintsValidator validator, IMapper mapper, Instance instance)
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateConstraintCommandValidator(validator, mapper, instance));
        }
    }
}