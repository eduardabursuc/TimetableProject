using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.CourseQueries
{
    public class GetCourseByIdQuery : IRequest<Result<CourseDto>>
    {
        public required Guid CourseId { get; init; }
    }
}