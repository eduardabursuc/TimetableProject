using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetCourseByNameQueryHandler : IRequestHandler<GetCourseByNameQuery, Result<CourseDTO>>
    {
        private readonly ICourseRepository repository;
        private readonly IMapper mapper;

        public GetCourseByNameQueryHandler(ICourseRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<CourseDTO>> Handle(GetCourseByNameQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByNameAsync(request.CourseName);

            if (!result.IsSuccess) return Result<CourseDTO>.Failure(result.ErrorMessage);

            var courseDTO = mapper.Map<CourseDTO>(result.Data);
            return Result<CourseDTO>.Success(courseDTO);
        }
    }
}