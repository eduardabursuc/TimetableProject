namespace Domain.Entities;

public class Event(string group, string eventName, string courseName, Guid professorId)
{
    public string Group { get; set; } = group;
    public string EventName { get; set; } = eventName;
    public string CourseName { get; set; } = courseName;
    public Guid ProfessorId { get; set; } = professorId;

    public HashSet<Constraint> Constraints { get; set; } = [];
}