namespace Domain.Entities
{
    public class Timetable
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        
        // Day and Time
        public required string Day { get; set; }
        public required string Time { get; set; }

        // Event Details
        public required string EventName { get; set; }
        public Event Event { get; set; } = null!;

        // Course Details
        public required string CourseName { get; set; }
        public int Credits { get; set; }
        public bool IsOptional { get; set; }

        // Group and Room
        public required string Group { get; set; }
        public required string RoomName { get; set; }
        public Room Room { get; set; } = null!;
    }
}