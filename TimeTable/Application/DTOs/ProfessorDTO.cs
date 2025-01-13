using Domain.Entities;

namespace Application.DTOs;

public class ProfessorDto
{
    public required string UserEmail { get; init; }
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}