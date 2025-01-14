namespace Domain.Entities
{
    public class Professor
    {
        public required string UserEmail { get; init; }
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
    }
}
