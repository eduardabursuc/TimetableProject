using System.Text.Json.Serialization;
namespace Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConstraintType
    {
        // Hard Constraints
        HARD_NO_OVERLAP,
        HARD_YEAR_PRIORITY,
        HARD_ROOM_CAPACITY,
        
        // Soft Constraints (Room)
        SOFT_ROOM_CHANGE,
        SOFT_ROOM_PREFERENCE,
        
        // Soft Constraints (Time)
        SOFT_TIME_CHANGE,
        SOFT_DAY_CHANGE,
        SOFT_ADD_WINDOW,
        SOFT_REMOVE_WINDOW,
        SOFT_DAY_OFF,
        
        // Soft Constraints (Structure)
        SOFT_WEEK_EVENNESS,
        SOFT_CONSECUTIVE_HOURS,
        SOFT_INTERVAL_AVAILABILITY,
        SOFT_INTERVAL_UNAVAILABILITY,
        SOFT_LECTURE_BEFORE_LABS
    }
    
    public class Constraint
    {
        public Guid Id { get; set; }
        public ConstraintType Type { get; set; }
        public Guid? ProfessorId { get; set; }
        public string? CourseName { get; set; }
        public string? RoomName { get; set; }
        public string? WantedRoomName { get; set; }
        public string? GroupName { get; set; }
        public string? Day { get; set; }
        public string? Time { get; set; }
        public string? WantedDay { get; set; }
        public string? WantedTime { get; set; }
        public string? Event { get; set; }
    }
}
