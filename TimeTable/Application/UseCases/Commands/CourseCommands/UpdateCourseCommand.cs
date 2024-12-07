using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.CourseCommands
{
    public class UpdateCourseCommand : CreateCourseCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}