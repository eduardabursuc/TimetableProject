using Domain.Entities;

namespace Application.DTOs;

public class TimetableDto
{
    public string UserEmail { get; init; }
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public List<Event> Events { get; set; } = [];
}