using Application.DTOs;
using Application.UseCases.Queries.RoomQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.RoomQueryHandlers
{
    public class GetRoomByIdQueryHandler(IRoomRepository repository, IMapper mapper)
        : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
    {
        public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);
    
            if (!result.IsSuccess) return Result<RoomDto>.Failure(result.ErrorMessage);
    
            var roomDto = mapper.Map<RoomDto>(result.Data);
            return Result<RoomDto>.Success(roomDto);
        }
    }
}

