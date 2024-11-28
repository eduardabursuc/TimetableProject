using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Event(string group, string eventName, string courseName, Guid professorId)
{
    public string Group { get; set; } = group;
    public string EventName { get; set; } = eventName;
    public string CourseName { get; set; } = courseName;
    public Guid ProfessorId { get; set; } = professorId;
    public bool WeekEvenness { get; set; } = false;
    
    [NotMapped]
    public string ProfessorName { get; set; } = "";
    [NotMapped]
    public int CourseCredits { get; set; } = 0;
    [NotMapped]
    public string CoursePackage { get; set; } = "";
}