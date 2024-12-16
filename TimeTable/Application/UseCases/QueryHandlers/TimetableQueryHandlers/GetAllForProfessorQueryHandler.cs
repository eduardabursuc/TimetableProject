using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.TimetableQueryHandlers
{
    public class GetAllForProfessorQueryHandler(ITimetableRepository repository, IMapper mapper)
        : IRequestHandler<GetAllForProfessorQuery, Result<List<TimetableDto>>>
    {
        public async Task<Result<List<TimetableDto>>> Handle(GetAllForProfessorQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllForProfessorAsync(request.ProfessorEmail);

            if (!result.IsSuccess) return Result<List<TimetableDto>>.Failure(result.ErrorMessage);
            
            var timetableDtOs = mapper.Map<List<TimetableDto>>(result.Data) ?? new List<TimetableDto>();
            return Result<List<TimetableDto>>.Success(timetableDtOs);
        }
    }
}