namespace Domain.Entities;

public class Timetable
{
    public Guid Id { get; set; }
    public List<Timeslot> Timeslots { get; set; } = [];
}