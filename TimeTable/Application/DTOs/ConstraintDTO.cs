using Domain.Entities;

namespace Application.DTOs
{
    public class ConstraintDto
    {
        public Guid TimetableId { get; init; }
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