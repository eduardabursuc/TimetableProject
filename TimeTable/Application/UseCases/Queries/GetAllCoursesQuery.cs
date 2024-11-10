using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetAllCoursesQuery : IRequest<Result<List<CourseDTO>>>
    {
    }
}