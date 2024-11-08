using FluentValidation;
using AutoMapper;
using Application.Validators;

namespace Application.UseCases.Commands
{
    public class UpdateConstraintCommandValidator : AbstractValidator<UpdateConstraintCommand>
    {
        public UpdateConstraintCommandValidator(ConstraintsValidator validator, IMapper mapper)
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateConstraintCommandValidator(validator, mapper));
        }
    }
}