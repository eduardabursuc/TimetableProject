using System.Text.Json.Serialization;
namespace Domain.Entities
{
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
        public Guid Id { get; set; }
        public ConstraintType Type { get; set; }
        public string ProfessorId { get; set; }
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
