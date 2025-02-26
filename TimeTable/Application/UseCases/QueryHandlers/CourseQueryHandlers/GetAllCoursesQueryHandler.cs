using Application.DTOs;
using Application.UseCases.Queries.CourseQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.CourseQueryHandlers
{
    public class GetAllCoursesQueryHandler(ICourseRepository repository, IMapper mapper)
        : IRequestHandler<GetAllCoursesQuery, Result<List<CourseDto>>>
    {
        public async Task<Result<List<CourseDto>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync(request.UserEmail);

            if (!result.IsSuccess) return Result<List<CourseDto>>.Failure(result.ErrorMessage);
            
            var courseDtOs = mapper.Map<List<CourseDto>>(result.Data) ?? [];
            return Result<List<CourseDto>>.Success(courseDtOs);
        }
    }
}