using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class CreateTimetableCommand : IRequest<Result<Guid>>
    {
        public required string UserEmail { get; set; }
        public required string Name { get; set; }
        public required List<Event> Events { get; init; } 
        public required List<Timeslot> Timeslots { get; set; } 

        public CreateTimetableCommand() { }
    }
}