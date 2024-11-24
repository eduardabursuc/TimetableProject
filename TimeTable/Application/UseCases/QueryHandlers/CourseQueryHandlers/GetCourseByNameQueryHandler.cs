using Application.DTOs;
using Application.UseCases.Queries.CourseQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.CourseQueryHandlers
{
    public class GetCourseByNameQueryHandler(ICourseRepository repository, IMapper mapper)
        : IRequestHandler<GetCourseByNameQuery, Result<CourseDto>>
    {
        public async Task<Result<CourseDto>> Handle(GetCourseByNameQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByNameAsync(request.CourseName);

            if (!result.IsSuccess) return Result<CourseDto>.Failure(result.ErrorMessage);

            var courseDto = mapper.Map<CourseDto>(result.Data);
            return Result<CourseDto>.Success(courseDto);
        }
    }
}