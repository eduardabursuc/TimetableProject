using Application.UseCases.Commands.CourseCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.CourseCommandHandlers
{
    public class UpdateCourseCommandHandler(ICourseRepository repository, IMapper mapper)
        : IRequestHandler<UpdateCourseCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = mapper.Map<Course>(request);
            var result = await repository.UpdateAsync(course);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}