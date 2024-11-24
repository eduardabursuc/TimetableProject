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
        public Guid Id { get; init; }
        public ConstraintType Type { get; init; }
        public Guid? ProfessorId { get; init; }
        public string? CourseName { get; init; }
        public string? RoomName { get; init; }
        public string? WantedRoomName { get; init; }
        public string? GroupName { get; init; }
        public string? Day { get; init; }
        public string? Time { get; init; }
        public string? WantedDay { get; init; }
        public string? WantedTime { get; init; }
        public string? Event { get; init; }
    }
}
