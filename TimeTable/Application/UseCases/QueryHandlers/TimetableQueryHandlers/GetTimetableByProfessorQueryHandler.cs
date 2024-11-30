using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.TimetableQueryHandlers
{
    public class GetTimetableByProfessorQueryHandler(ITimetableRepository repository, IMapper mapper)
        : IRequestHandler<GetTimetableByProfessorQuery, Result<TimetableDto>>
    {
        public async Task<Result<TimetableDto>> Handle(GetTimetableByProfessorQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByProfessorAsync(request.Id, request.ProfessorId);
    
            if (!result.IsSuccess) return Result<TimetableDto>.Failure(result.ErrorMessage);
    
            var timetableDto = mapper.Map<TimetableDto>(result.Data);
            return Result<TimetableDto>.Success(timetableDto);
        }
    }
}