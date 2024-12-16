using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Event
{
    public Guid? TimetableId { get; set; }
    public Guid? Id { get; set; }
    
    public required string EventName { get; set; }
    public required Guid CourseId { get; set; }
    public required Guid ProfessorId { get; set; }
    public required Guid GroupId { get; set; }
    public required int Duration { get; set; }
    public required bool isEven { get; set; } = false;
    
    public Guid? RoomId { get; set; }
    public Timeslot? Timeslot { get; set; } = new Timeslot { Day = "", Time = "" };
    
    public Event() { Id = Guid.NewGuid(); }
    
    [JsonConstructor]
    public Event(string eventName, Guid courseId, Guid professorId, Guid groupId, int duration)
    {
        Id = Guid.NewGuid();
        EventName = eventName;
        CourseId = courseId;
        ProfessorId = professorId;
        GroupId = groupId;
        Duration = duration;
    }
}