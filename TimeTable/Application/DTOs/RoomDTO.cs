namespace Application.DTOs
{
    public class RoomDto
    {
        public required string UserEmail { get; init; }
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public int Capacity { get; init; }
    }
}
