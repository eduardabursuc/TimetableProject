using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetTimetableByProfessorQueryValidator : AbstractValidator<GetTimetableByProfessorQuery>
{
    public GetTimetableByProfessorQueryValidator(Instance instance)
    {
        RuleFor(t => t.Id).NotEmpty();

        RuleFor(t => t.ProfessorId)
            .NotEmpty()
            .Must(
                professorId => instance.Professors.Any(p => p.Id == professorId)) 
            .WithMessage("Room does not exist.");
    }
}