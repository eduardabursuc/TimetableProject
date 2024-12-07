namespace Application.DTOs
{
    public class GroupDto
    {
        public required string UserEmail { get; init; }
        public Guid Id { get; init; }
        public required string Name { get; init; }
    }
}
