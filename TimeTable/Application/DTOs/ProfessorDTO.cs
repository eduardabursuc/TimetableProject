namespace Application.DTOs
{
    public class ProfessorDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> CourseNames { get; set; } = new List<string>(); 
    }
}
