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
}
