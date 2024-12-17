using System.Text.Json.Serialization;
namespace Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConstraintType
    {
        // Hard Constraints
        HARD_NO_OVERLAP = 0,
        HARD_YEAR_PRIORITY = 1,
        HARD_ROOM_CAPACITY = 2,
        
        // Soft Constraints (Room)
        SOFT_ROOM_CHANGE = 3,
        SOFT_ROOM_PREFERENCE = 4,
        
        // Soft Constraints (Time)
        SOFT_TIME_CHANGE = 5,
        SOFT_DAY_CHANGE = 6,
        SOFT_ADD_WINDOW = 7,
        SOFT_REMOVE_WINDOW = 8,
        SOFT_DAY_OFF = 9, 
        
        // Soft Constraints (Structure)
        SOFT_WEEK_EVENNESS = 10,
        SOFT_CONSECUTIVE_HOURS = 11,
        SOFT_INTERVAL_AVAILABILITY = 12,
        SOFT_INTERVAL_UNAVAILABILITY = 13,
        SOFT_LECTURE_BEFORE_LABS = 14
    }
    
    public class Constraint
    {
        public Guid TimetableId { get; init; }
        public Guid Id { get; init; }
        public ConstraintType Type { get; init; }
        public Guid? ProfessorId { get; init; }
        public Guid? CourseId { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? WantedRoomId { get; set; }
        public Guid? GroupId { get; set; }
        public string? Day { get; init; }
        public string? Time { get; init; }
        public string? WantedDay { get; init; }
        public string? WantedTime { get; init; }
        public string? Event { get; init; }
    }
}
