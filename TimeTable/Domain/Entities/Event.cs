using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Event(string group, string eventName, string courseName, Guid professorId)
{
    public string Group { get; set; } = group;
    public string EventName { get; set; } = eventName;
    public string CourseName { get; set; } = courseName;
    public Guid ProfessorId { get; set; } = professorId;
    public bool WeekEvenness { get; set; } = false;
    public string ProfessorName { get; set; } = "";
    public int CourseCredits { get; set; } = 0;
    public string CoursePackage { get; set; } = "";
    public int TimeInterval { get; set; } = 1;
}