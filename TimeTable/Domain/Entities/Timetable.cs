namespace Domain.Entities;

public class Timetable
{
    public string UserEmail { get; set; }
    public Guid Id { get; init; }
    public string Name { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public bool IsPublic { get; set; } = false;
    public List<Event> Events { get; set; } = [];
}