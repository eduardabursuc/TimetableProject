using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetTimetableByRoomQueryValidator : AbstractValidator<GetTimetableByRoomQuery>
{
    public GetTimetableByRoomQueryValidator(Instance instance)
    {
        RuleFor(t => t.Id).NotEmpty();

        RuleFor(t => t.RoomName)
            .NotEmpty()
            .Must(
                roomName => instance.Rooms.Any(r => r.Name == roomName)) 
            .WithMessage("Room does not exist.");
    }
}

