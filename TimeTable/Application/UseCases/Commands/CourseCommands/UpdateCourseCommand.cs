using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.CourseCommands
{
    public class UpdateCourseCommand : CreateCourseCommand, IRequest<Result<string>>
    {
    }
}