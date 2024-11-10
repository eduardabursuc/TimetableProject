using Domain.Entities;

namespace Application.DTOs;

public class ProfessorDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<Course> Courses { get; set; } = new List<Course>();
}