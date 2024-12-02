using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class UpdateTimetableCommand : IRequest<Result<Unit>>
    {
        public required Guid Id { get; init; }
        public required List<Timeslot> Timeslots { get; init; }
    }
}

