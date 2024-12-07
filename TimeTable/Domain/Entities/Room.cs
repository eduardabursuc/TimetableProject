namespace Domain.Entities
{
    public class Room
    {
        public required string UserEmail { get; init; }
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public int Capacity { get; init; }
    }
}
