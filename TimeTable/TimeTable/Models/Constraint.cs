namespace TimeTable.Models;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConstraintType
{
    ROOM_CHANGE,
    ROOM_PREFERENCE,
    TIME_CHANGE,
    DAY_CHANGE,
    INTERVAL_AVAILABILITY,
    INTERVAL_UNAVAILABILITY,
    WEEK_EVENNESS,
    ADD_WINDOW,
    REMOVE_WINDOW,
    DAY_OFF,
    CONSECUTIVE_HOURS,
    LECTURE_BEFORE_LABS
}
    

public class Constraint
{
    public ConstraintType Type { get; set; }
    public Professor Professor { get; set; }
    public Course Course { get; set; }
    public Room Room { get; set; }
    public Room WantedRoom { get; set; }
    public List<Timeslot> Timeslots { get; set; }
    public Timeslot WantedTimeslot { get; set; }
    public Group Group { get; set; }
    public string Event { get; set; }

    public Constraint() { }
    
    public Constraint(ConstraintType type, Professor professor = null, Course course = null, Room room = null,
        Room wanted_room = null, List<Timeslot> timeslots = null, Timeslot wanted_timeslot = null,
        Group group = null, string _event = null)
    {
        Type = type;
        Professor = professor;
        Course = course;
        Room = room;
        WantedRoom = wanted_room;
        Timeslots = timeslots;
        WantedTimeslot = wanted_timeslot;
        Group = group;
        Event = _event;
    }
    
}