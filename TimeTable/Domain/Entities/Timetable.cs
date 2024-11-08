namespace Domain.Entities;

public class Timetable
{
    List<Timeslot> Timeslots { get; set; }
    
    public Timetable()
    {
        Timeslots = new List<Timeslot>();
    }
    
}