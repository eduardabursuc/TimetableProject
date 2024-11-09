using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetCourseByNameQuery : IRequest<Result<CourseDTO>>
    {
        public string CourseName { get; set; }
    }
}