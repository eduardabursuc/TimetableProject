using Domain.Entities;

namespace Application.DTOs;

public class TimetableDto
{
    public List<Timeslot> Timeslots { get; set; } = [];
}