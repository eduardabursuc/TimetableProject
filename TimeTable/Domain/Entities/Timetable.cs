namespace Domain.Entities;

public class Timetable
{
    public string UserEmail { get; init; }
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Timeslot> Timeslots { get; set; } = [];
}