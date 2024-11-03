using Domain.Entities;
using MediatR;

public class UpdateConstraintCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public ConstraintType Type { get; set; }
    public string? ProfessorId { get; set; }
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