using Application.DTOs;
using Domain.Entities;

public class ConstraintDTO
{
    public Guid Id { get; set; }
    public ConstraintType Type { get; set; }
    public Guid ProfessorId { get; set; }
    public Guid CourseId { get; set; }
    public Guid RoomId { get; set; }
    public Guid? WantedRoomId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? WantedTimeslotId { get; set; }
    public List<TimeslotDTO> Timeslots { get; set; } = new List<TimeslotDTO>();
    public string Event { get; set; }
}
