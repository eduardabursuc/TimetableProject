using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Timeslot(string day, string time, string? roomName)
    {
        public required string Day { get; init; } = day;
        public required string Time { get; init; } = time;
        public bool IsAvailable { get; set; } = true;
        [NotMapped]
        public Event Event { get; set; } = new Event("", "", "", Guid.Empty);
        public string? RoomName { get; set; } = roomName;

        public Timeslot(string day, string time) : this(day, time, null)
        {
        }

        public Timeslot() : this(string.Empty, string.Empty, null)
        {
        }
    }
}