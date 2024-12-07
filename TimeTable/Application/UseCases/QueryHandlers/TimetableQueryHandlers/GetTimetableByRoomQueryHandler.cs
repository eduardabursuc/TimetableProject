using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.TimetableQueryHandlers
{
    public class GetTimetableByRoomQueryHandler(ITimetableRepository repository, IMapper mapper)
        : IRequestHandler<GetTimetableByRoomQuery, Result<TimetableDto>>
    {
        public async Task<Result<TimetableDto>> Handle(GetTimetableByRoomQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByRoomAsync(request.Id, request.RoomId);
    
            if (!result.IsSuccess) return Result<TimetableDto>.Failure(result.ErrorMessage);
    
            var timetableDto = mapper.Map<TimetableDto>(result.Data);
            return Result<TimetableDto>.Success(timetableDto);
        }
    }
}