using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.TimetableCommands
{
    public abstract class UpdateTimetableCommandValidator : AbstractValidator<UpdateTimetableCommand>
    {
        protected UpdateTimetableCommandValidator(IMapper mapper, Instance instance)
        {
            RuleFor(t => t.Timetable).NotEmpty();
        }
    }
}
