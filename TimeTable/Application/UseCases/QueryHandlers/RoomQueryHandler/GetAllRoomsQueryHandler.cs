using Application.DTOs;
using Application.UseCases.Queries.RoomQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.RoomQueryHandlers
{
    public class GetAllRoomsQueryHandler(IRoomRepository repository, IMapper mapper)
        : IRequestHandler<GetAllRoomsQuery, Result<List<RoomDto>>>
    {
        public async Task<Result<List<RoomDto>>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync();

            if (!result.IsSuccess) return Result<List<RoomDto>>.Failure(result.ErrorMessage);

            var groupDtOs = mapper.Map<List<RoomDto>>(result.Data) ?? [];
            return Result<List<RoomDto>>.Success(groupDtOs);
        }
    }
}