using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.TimetableQueryHandlers
{
    public class GetTimetableByIdQueryHandler(ITimetableRepository repository, IMapper mapper)
        : IRequestHandler<GetTimetableByIdQuery, Result<TimetableDto>>
    {
        public async Task<Result<TimetableDto>> Handle(GetTimetableByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);

            if (!result.IsSuccess) return Result<TimetableDto>.Failure(result.ErrorMessage);

            var timetableDto = mapper.Map<TimetableDto>(result.Data);
            return Result<TimetableDto>.Success(timetableDto);
        }
    }
}