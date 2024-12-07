using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.RoomCommands
{
    public record DeleteRoomCommand(Guid Id) : IRequest<Result<Unit>>;
}


