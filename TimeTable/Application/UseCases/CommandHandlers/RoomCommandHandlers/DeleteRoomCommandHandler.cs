using Application.UseCases.Commands.RoomCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.RoomCommandHandlers
{
    public class DeleteRoomCommandHandler(IRoomRepository repository, IMapper mapper)
        : IRequestHandler<DeleteRoomCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var room = mapper.Map<Room>(request);
            var result = await repository.DeleteAsync(room.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}