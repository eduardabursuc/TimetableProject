using FluentValidation;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public class CreateProfessorCommandValidator : AbstractValidator<CreateProfessorCommand>
    {
        public CreateProfessorCommandValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(c => c.UserEmail)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
        }
    }
}