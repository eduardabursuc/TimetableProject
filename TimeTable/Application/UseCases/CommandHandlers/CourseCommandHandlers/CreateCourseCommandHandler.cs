using Application.UseCases.Commands.CourseCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.CourseCommandHandlers
{
    public class CreateCourseCommandHandler(ICourseRepository repository, IMapper mapper)
        : IRequestHandler<CreateCourseCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = mapper.Map<Course>(request);
            var result = await repository.AddAsync(course);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}