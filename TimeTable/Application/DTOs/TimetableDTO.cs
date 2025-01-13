using Domain.Entities;

namespace Application.DTOs;

public class TimetableDto
{
    public required string UserEmail { get; init; }
    public Guid? Id { get; set; }
    public required string Name { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public bool IsPublic { get; set; } = false;
    public List<Event> Events { get; set; } = [];
}