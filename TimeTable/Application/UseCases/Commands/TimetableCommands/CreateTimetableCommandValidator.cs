using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentValidation;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class CreateTimetableCommandValidator : AbstractValidator<CreateTimetableCommand>
    {
        public CreateTimetableCommandValidator(IMapper mapper, IGroupRepository groupRepository, ICourseRepository courseRepository, IProfessorRepository professorRepository)
        {
            RuleForEach(c => c.Events)
                .Must(e => 
                    groupRepository.GetByIdAsync(e.GroupId).Result.IsSuccess &&
                    courseRepository.GetByIdAsync(e.CourseId).Result.IsSuccess &&
                    professorRepository.GetByIdAsync(e.ProfessorId).Result.IsSuccess
                    )
                .WithMessage(e => $"Event validation failed.");
        }
    }
}