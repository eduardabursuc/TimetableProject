namespace Application.DTOs;

public class UserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string AccountType { get; init; }
}