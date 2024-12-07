using Domain.Entities;

namespace Application.DTOs;

public class TimetableDto
{
    public string UserEmail { get; init; }
    public Guid? Id { get; set; }
    public List<Event> Events { get; set; } = [];
}