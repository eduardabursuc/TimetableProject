using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, Result<List<CourseDTO>>>
    {
        private readonly ICourseRepository repository;
        private readonly IMapper mapper;

        public GetAllCoursesQueryHandler(ICourseRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<List<CourseDTO>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync();

            if (!result.IsSuccess) return Result<List<CourseDTO>>.Failure(result.ErrorMessage);
            
            var courseDTOs = mapper.Map<List<CourseDTO>>(result.Data) ?? new List<CourseDTO>();
            return Result<List<CourseDTO>>.Success(courseDTOs);
        }
    }
}