namespace TimeTable.Models;
using System.Text.Json;

public class Course
{
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public string Package { get; set; }
    public int Semester { get; set; }
    public string Level { get; set; }

    public Course(string courseName, int credits, string package, int semester, string level)
    {
        CourseName = courseName;
        Credits = credits;
        Package = package;
        Semester = semester;
        Level = level;
    }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}