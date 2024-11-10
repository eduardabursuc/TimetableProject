using Domain.Entities;

namespace Application.DTOs;

public class CourseDto
{
    public required string CourseName { get; set; }
    public required int Credits { get; set; }
    public required string Package { get; set; }
    public required int Semester { get; set; }
    public required string Level { get; set; }
    public List<Professor> Professors { get; set; } = new List<Professor>();
}