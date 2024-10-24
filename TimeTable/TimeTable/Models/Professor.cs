namespace TimeTable.Models;
using System.Text.Json;

public class Professor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Course> Courses { get; set; }
    public List<Constraint> Constraints { get; set; }

    public Professor(string name, List<Course> courses)
    {
        Name = name;
        Courses = courses;
        Constraints = new List<Constraint>();
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}