using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public abstract class GetTimetableByIdQueryValidator : AbstractValidator<GetTimetableByIdQuery>
{
    protected GetTimetableByIdQueryValidator()
    {
        RuleFor(t => t.Id).NotEmpty();
    }
}