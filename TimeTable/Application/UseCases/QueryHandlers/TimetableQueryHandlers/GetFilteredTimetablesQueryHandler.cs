using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using Application.Utils;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using Gridify;
using MediatR;

namespace Application.UseCases.QueryHandlers.TimetableQueryHandlers
{
    public class GetFilteredTimetablesQueryHandler(ITimetableRepository repository, IMapper mapper)
        : IRequestHandler<GetFilteredTimetablesQuery, Result<PagedResult<TimetableDto>>>
    {
        public async Task<Result<PagedResult<TimetableDto>>> Handle(GetFilteredTimetablesQuery request, CancellationToken cancellationToken)
        {
            var timetables = await repository.GetAllAsync(request.UserEmail);
            var timetablesList = timetables.Data;
            var query = timetablesList.AsQueryable();
    
            // Apply paging
            var pagedTimetables = query.ApplyPaging(request.Page, request.PageSize);
            var timetableDtos = mapper.Map<List<TimetableDto>>(pagedTimetables);
    
            var pagedResult = new PagedResult<TimetableDto>(timetableDtos, query.Count());
    
            return Result<PagedResult<TimetableDto>>.Success(pagedResult);
        }
    }
}

