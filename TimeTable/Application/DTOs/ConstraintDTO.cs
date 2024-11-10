using Domain.Entities;

namespace Application.DTOs
{
    public class ConstraintDto
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
