using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.TimetableCommands
{
    public abstract class UpdateTimetableCommandValidator : AbstractValidator<UpdateTimetableCommand>
    {
        protected UpdateTimetableCommandValidator(IMapper mapper)
        {
            RuleFor(t => t.Id).NotEmpty();
            RuleFor(t => t.Events).NotEmpty();
        }
    }
}
