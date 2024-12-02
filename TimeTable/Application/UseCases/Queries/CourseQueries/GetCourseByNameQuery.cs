using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.CourseQueries
{
    public class GetCourseByNameQuery : IRequest<Result<CourseDto>>
    {
        public required string CourseName { get; init; }
    }
}