using Application.UseCases.Commands.RoomCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.RoomCommandHandlers
{
    public class UpdateRoomCommandHandler(IRoomRepository repository, IMapper mapper)
        : IRequestHandler<UpdateRoomCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = mapper.Map<Room>(request);
            var result = await repository.UpdateAsync(room);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}