using FluentValidation;

namespace Application.UseCases.Commands
{
    public class UpdateToDoItemCommandValidator : AbstractValidator<UpdateToDoItemCommand>
    {
        public UpdateToDoItemCommandValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateToDoItemCommandValidator());
        }
    }
}