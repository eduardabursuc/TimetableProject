using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.TimetableQueryHandlers
{
    public class GetTimetableByGroupQueryHandler(ITimetableRepository repository, IMapper mapper)
        : IRequestHandler<GetTimetableByGroupQuery, Result<TimetableDto>>
    {
        public async Task<Result<TimetableDto>> Handle(GetTimetableByGroupQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByGroupAsync(request.Id, request.GroupName);
    
            if (!result.IsSuccess) return Result<TimetableDto>.Failure(result.ErrorMessage);
    
            var timetableDto = mapper.Map<TimetableDto>(result.Data);
            return Result<TimetableDto>.Success(timetableDto);
        }
    }
}

