using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.RoomCommands
{
    public class UpdateRoomCommand : CreateRoomCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}
