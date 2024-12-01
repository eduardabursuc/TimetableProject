using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class CreateTimetableCommandValidator : AbstractValidator<CreateTimetableCommand>
    {
        public CreateTimetableCommandValidator(IMapper mapper, Instance instance)
        {
            RuleForEach(c => c.Events)
                .Must(e => ValidateEvent(e, instance))
                .WithMessage(e => $"Event validation failed.");
        }

        private static bool ValidateEvent(Event e, Instance instance)
        {
            // Check if the Group exists in instance.Groups
            var groupValid = instance.Groups.Any(g => g.Name == e.Group);
            if (!groupValid) return false;

            // Check if the CourseName exists in instance.Courses
            var courseValid = instance.Courses.Any(c => c.CourseName == e.CourseName);
            if (!courseValid) return false;

            // Check if the ProfessorId exists in instance.Professors
            var professorValid = instance.Professors.Any(p => p.Id == e.ProfessorId);
            return professorValid;
        }
    }
}