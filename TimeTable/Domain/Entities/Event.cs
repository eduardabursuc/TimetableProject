using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Event(Guid groupId, string group, string eventName, Guid courseId, string courseName, Guid professorId)
{
    public string EventName { get; set; } = eventName;
    public Guid CourseId { get; set; } = courseId;
    public Guid ProfessorId { get; set; } = professorId;
    public Guid GroupId { get; set; } = groupId;
    public string Group { get; set; } = group;
    public string CourseName { get; set; } = courseName;
    public bool WeekEvenness { get; set; } = false;
    public string ProfessorName { get; set; } = "";
    public int CourseCredits { get; set; } = 0;
    public string CoursePackage { get; set; } = "";
    public int TimeInterval { get; set; } = 1;
}