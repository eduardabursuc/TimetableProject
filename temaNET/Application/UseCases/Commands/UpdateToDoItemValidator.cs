using FluentValidation;

namespace Application.UseCases.Commands
{
    public class UpdateToDoItemCommandValidator : AbstractValidator<UpdateToDoItemCommand>
    {
        public UpdateToDoItemCommandValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
            RuleFor(t => t.Description).NotEmpty().MaximumLength(200);
            RuleFor(t => t.DueDate).NotEmpty();
            RuleFor(t => t.IsDone).NotNull();
        }
    }
}