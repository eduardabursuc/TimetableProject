using Application.UseCases.Commands.CourseCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.CourseCommandHandlers
{
    public class CreateCourseCommandHandler(ICourseRepository repository, IMapper mapper)
        : IRequestHandler<CreateCourseCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = mapper.Map<Course>(request);
            var result = await repository.AddAsync(course);
            return result.IsSuccess ? Result<string>.Success(result.Data) : Result<string>.Failure(result.ErrorMessage);
        }
    }
}