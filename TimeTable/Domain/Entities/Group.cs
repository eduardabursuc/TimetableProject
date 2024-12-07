namespace Domain.Entities
{
    public class Group
    {
        public required string UserEmail { get; init; }
        public Guid Id { get; init; }
        public required string Name { get; init; }
    }
}
