using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Timeslot
    {
        public required string Day { get; set; }
        public required string Time { get; set; }
        public bool IsAvailable { get; set; }
        [NotMapped]
        public Event Event { get; set; } = new Event("", "", "", Guid.Empty);
        public string? RoomName { get; set; }

        public Timeslot(string day, string time)
        {
            Day = day;
            Time = time;
            IsAvailable = true;
        }

        public Timeslot(string day, string time, string? roomName)
        {
            Day = day;
            Time = time;
            RoomName = roomName;
            IsAvailable = true;
        }

        public Timeslot()
        {
            Day = string.Empty;
            Time = string.Empty;
        }
    }
}
