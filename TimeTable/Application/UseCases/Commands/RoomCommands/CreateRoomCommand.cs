using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.RoomCommands
{
    public class CreateRoomCommand : IRequest<Result<Guid>>
    {
        public required string UserEmail { get; init; }
        public required string Name { get; init; }
        public required int Capacity { get; init; }

        public CreateRoomCommand() { }
        
    }
}