namespace Domain.Entities;

public class Event
{
    public string Group { get; set; }
    public string EventName { get; set; }
    public string CourseName { get; set; }
    public Guid ProfessorId { get; set; }
    
    public HashSet<Constraint> Constraints { get; set; } = new HashSet<Constraint>();
    public HashSet<Timeslot> Timeslots { get; set; } = new HashSet<Timeslot>();

    public Event(string group, string eventName, string courseName, Guid professorId)
    {
        EventName = eventName;
        CourseName = courseName;
        Group = group;
        ProfessorId = professorId;
    }

}