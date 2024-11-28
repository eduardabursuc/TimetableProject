using Domain.Entities;

namespace Application.DTOs;

public class TimetableDto
{
    public Guid Id { get; set; }
    public List<Timeslot> Timeslots { get; set; } = [];
}