using Domain.Entities;

namespace Application.DTOs;

public class CourseDTO
{
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public string Package { get; set; }
    public int Semester { get; set; }
    public string Level { get; set; }
    public List<Professor> Professors { get; set; } = new List<Professor>();
}