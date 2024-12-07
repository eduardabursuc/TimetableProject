using Domain.Entities;
using Domain.Repositories;
using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetTimetableByProfessorQueryValidator : AbstractValidator<GetTimetableByProfessorQuery>
{
    public GetTimetableByProfessorQueryValidator(IProfessorRepository repository)
    {
        RuleFor(t => t.Id).NotEmpty();

        RuleFor(t => t.ProfessorId)
            .NotEmpty()
            .Must(
                professorId => repository.GetByIdAsync(professorId).Result.IsSuccess )
            .WithMessage("Room does not exist.");
    }
}