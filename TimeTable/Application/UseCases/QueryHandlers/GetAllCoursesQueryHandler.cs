using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, Result<List<CourseDto>>>
    {
        private readonly ICourseRepository repository;
        private readonly IMapper mapper;

        public GetAllCoursesQueryHandler(ICourseRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<List<CourseDto>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync();

            if (!result.IsSuccess) return Result<List<CourseDto>>.Failure(result.ErrorMessage);
            
            var courseDTOs = mapper.Map<List<CourseDto>>(result.Data) ?? new List<CourseDto>();
            return Result<List<CourseDto>>.Success(courseDTOs);
        }
    }
}