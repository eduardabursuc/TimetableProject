using Application.UseCases.Commands.RoomCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.RoomCommandHandlers
{
    public class CreateRoomCommandHandler(IRoomRepository repository, IMapper mapper)
        : IRequestHandler<CreateRoomCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = mapper.Map<Room>(request);
            var result = await repository.AddAsync(room);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}