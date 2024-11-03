namespace Application.DTOs
{
    public class ProfessorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> Courses { get; set; } = new List<Guid>(); // Initialize to avoid null references
    }
}
