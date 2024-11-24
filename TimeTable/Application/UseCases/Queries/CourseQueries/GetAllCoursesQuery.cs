using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.CourseQueries
{
    public class GetAllCoursesQuery : IRequest<Result<List<CourseDto>>>
    {
    }
}