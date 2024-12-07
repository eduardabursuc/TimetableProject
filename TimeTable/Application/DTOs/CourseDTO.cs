using Domain.Entities;

namespace Application.DTOs;

public class CourseDto
{
    public required string UserEmail { get; init; }
    public required Guid Id { get; init; }
    public required string CourseName { get; init; }
    public required int Credits { get; init; }
    public required string Package { get; init; }
    public required int Semester { get; init; }
    public required string Level { get; init; }
}