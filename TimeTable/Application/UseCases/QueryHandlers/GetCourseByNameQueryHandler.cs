using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetCourseByNameQueryHandler : IRequestHandler<GetCourseByNameQuery, Result<CourseDto>>
    {
        private readonly ICourseRepository repository;
        private readonly IMapper mapper;

        public GetCourseByNameQueryHandler(ICourseRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<CourseDto>> Handle(GetCourseByNameQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByNameAsync(request.CourseName);

            if (!result.IsSuccess) return Result<CourseDto>.Failure(result.ErrorMessage);

            var courseDTO = mapper.Map<CourseDto>(result.Data);
            return Result<CourseDto>.Success(courseDTO);
        }
    }
}