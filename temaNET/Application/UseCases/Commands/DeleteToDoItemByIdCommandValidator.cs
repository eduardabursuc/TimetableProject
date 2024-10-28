using FluentValidation;

namespace Application.UseCases.Commands
{
    public class DeleteToDoItemByIdCommandValidator : AbstractValidator<DeleteToDoItemByIdCommand>
    {
        public DeleteToDoItemByIdCommandValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
        }
    }
}