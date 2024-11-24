using Domain.Entities;

namespace Application.DTOs;

public class ProfessorDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}