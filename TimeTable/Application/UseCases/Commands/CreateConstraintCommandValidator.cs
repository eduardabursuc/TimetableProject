using FluentValidation;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        public CreateConstraintCommandValidator()
        {
            RuleFor(c => c.Type)
                .IsInEnum()
                .WithMessage("Constraint type is required.");
            
        }
    }
}
