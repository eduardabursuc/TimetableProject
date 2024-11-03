using FluentValidation;

namespace Application.UseCases.Commands
{
    public class UpdateConstraintCommandValidator : AbstractValidator<UpdateConstraintCommand>
    {
        public UpdateConstraintCommandValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
            RuleFor(t => t.Type).IsInEnum();
        }
    }
}