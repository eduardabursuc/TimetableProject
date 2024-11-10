using Application.UseCases.Commands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result<Unit>>
    {
        private readonly ICourseRepository repository;
        private readonly IMapper mapper;
        
        public DeleteCourseCommandHandler(ICourseRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = mapper.Map<Course>(request);
            var result = await repository.DeleteAsync(course.CourseName);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}