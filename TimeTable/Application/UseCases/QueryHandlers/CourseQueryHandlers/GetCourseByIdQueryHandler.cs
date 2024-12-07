using Application.DTOs;
using Application.UseCases.Queries.CourseQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.CourseQueryHandlers
{
    public class GetCourseByIdQueryHandler(ICourseRepository repository, IMapper mapper)
        : IRequestHandler<GetCourseByIdQuery, Result<CourseDto>>
    {
        public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.CourseId);

            if (!result.IsSuccess) return Result<CourseDto>.Failure(result.ErrorMessage);

            var courseDto = mapper.Map<CourseDto>(result.Data);
            return Result<CourseDto>.Success(courseDto);
        }
    }
}