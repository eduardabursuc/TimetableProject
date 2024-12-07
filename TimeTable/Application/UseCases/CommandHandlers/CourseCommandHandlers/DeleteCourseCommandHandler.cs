using Application.UseCases.Commands.CourseCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.CourseCommandHandlers
{
    public class DeleteCourseCommandHandler(ICourseRepository repository, IMapper mapper)
        : IRequestHandler<DeleteCourseCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = mapper.Map<Course>(request);
            var result = await repository.DeleteAsync(course.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}