using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Common;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result<string>>
    {
        private readonly ICourseRepository repository;
        private readonly IMapper mapper;

        public UpdateCourseCommandHandler(ICourseRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<string>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = mapper.Map<Course>(request);
            var result = await repository.UpdateAsync(course);
            if (result.IsSuccess)
            {
                return Result<string>.Success(result.Data);
            }

            return Result<string>.Failure(result.ErrorMessage);
        }
    }
}