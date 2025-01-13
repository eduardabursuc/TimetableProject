using FluentValidation;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public abstract class UpdateProfessorCommandValidator : AbstractValidator<UpdateProfessorCommand>
    {
        protected UpdateProfessorCommandValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateProfessorCommandValidator());
        }
    }
}