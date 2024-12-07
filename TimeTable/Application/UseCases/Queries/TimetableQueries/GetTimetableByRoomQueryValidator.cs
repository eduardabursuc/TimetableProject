using Domain.Entities;
using Domain.Repositories;
using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetTimetableByRoomQueryValidator : AbstractValidator<GetTimetableByRoomQuery>
{
    public GetTimetableByRoomQueryValidator(IRoomRepository repository)
    {
        RuleFor(t => t.Id).NotEmpty();
    }
}

