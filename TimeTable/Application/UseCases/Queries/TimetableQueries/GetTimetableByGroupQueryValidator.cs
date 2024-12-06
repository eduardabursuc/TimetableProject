using Domain.Repositories;
using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetTimetableByGroupQueryValidator : AbstractValidator<GetTimetableByGroupQuery>
{
    public GetTimetableByGroupQueryValidator(IGroupRepository repository)
    {
        RuleFor(t => t.Id).NotEmpty();

        RuleFor(t => t.GroupName)
            .NotEmpty()
            .Must(
                groupName => repository.GetByNameAsync(groupName).Result.IsSuccess )
            .WithMessage("Group does not exist.");
    }
}