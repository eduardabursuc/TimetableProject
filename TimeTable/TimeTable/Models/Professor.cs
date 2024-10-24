namespace TimeTable.Models;
using System.Text.Json;

public class Professor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Course> Courses { get; set; }

    public Professor(string name, List<Course> courses)
    {
        Id = Guid.NewGuid();
        Name = name;
        Courses = courses;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}