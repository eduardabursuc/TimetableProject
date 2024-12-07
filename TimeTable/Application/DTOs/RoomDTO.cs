namespace Application.DTOs
{
    public class RoomDto
    {
        public required string UserEmail { get; init; }
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required int Capacity { get; init; }
    }
}
