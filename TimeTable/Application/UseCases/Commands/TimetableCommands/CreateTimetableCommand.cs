using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class CreateTimetableCommand(List<Event> events) : IRequest<Result<Guid>>
    {
        public required List<Event> Events { get; set; } = events;

        public CreateTimetableCommand() : this(new List<Event>())
        {
        }
    }
}