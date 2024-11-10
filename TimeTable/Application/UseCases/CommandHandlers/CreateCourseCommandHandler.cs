using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Common;
using MediatR;
using Domain.Repositories;


namespace Application.UseCases.CommandHandlers
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<string>>
    {
        private readonly ICourseRepository repository;
        private readonly IMapper mapper;

        public CreateCourseCommandHandler(ICourseRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<string>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = mapper.Map<Course>(request);
            var result = await repository.AddAsync(course);
            if (result.IsSuccess)
            {
                return Result<string>.Success(result.Data);
            }

            return Result<string>.Failure(result.ErrorMessage);
        }
    }
}