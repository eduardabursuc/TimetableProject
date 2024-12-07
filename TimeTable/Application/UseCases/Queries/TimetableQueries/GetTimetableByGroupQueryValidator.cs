using Domain.Repositories;
using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetTimetableByGroupQueryValidator : AbstractValidator<GetTimetableByGroupQuery>
{
    public GetTimetableByGroupQueryValidator(IGroupRepository repository)
    {
        RuleFor(t => t.Id).NotEmpty();
    }
}