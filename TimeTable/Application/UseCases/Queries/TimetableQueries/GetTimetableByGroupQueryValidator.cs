using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetTimetableByGroupQueryValidator : AbstractValidator<GetTimetableByGroupQuery>
{
    public GetTimetableByGroupQueryValidator(Instance instance)
    {
        RuleFor(t => t.Id).NotEmpty();

        RuleFor(t => t.GroupName)
            .NotEmpty()
            .Must(
                groupName => instance.Groups.Any(g => g.Name == groupName)) 
            .WithMessage("Group does not exist.");
    }
}