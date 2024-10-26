using FluentValidation;

namespace Application.UseCases.Commands
{
    public class CreateToDoItemCommandValidator : AbstractValidator<CreateToDoItemCommand>
    {
        public CreateToDoItemCommandValidator()
        {
            RuleFor(t => t.Description).NotEmpty().MaximumLength(200);
            RuleFor(t => t.DueDate).NotEmpty();
            RuleFor(t => t.IsDone).NotNull();
        }
    }
}
