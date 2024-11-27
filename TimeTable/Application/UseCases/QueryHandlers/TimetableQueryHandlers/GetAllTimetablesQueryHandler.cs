using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.TimetableQueryHandlers;

public class GetAllTimetablesQueryHandler(ITimetableRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTimetablesQuery, Result<List<TimetableDto>>>
{
    public async Task<Result<List<TimetableDto>>> Handle(GetAllTimetablesQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAsync();

        if (!result.IsSuccess) return Result<List<TimetableDto>>.Failure(result.ErrorMessage);
            
        var timeTableDtOs = mapper.Map<List<TimetableDto>>(result.Data) ?? new List<TimetableDto>();
        return Result<List<TimetableDto>>.Success(timeTableDtOs);
    }
}