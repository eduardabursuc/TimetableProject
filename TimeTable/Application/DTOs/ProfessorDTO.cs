using Domain.Entities;

namespace Application.DTOs;

public class ProfessorDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Course> Courses { get; set; } = new List<Course>();
}