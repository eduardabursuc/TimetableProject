using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class UpdateTimetableCommand : IRequest<Result<Unit>>
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required bool IsPublic { get; init; }
        public required List<Event> Events { get; init; }
    }
}

