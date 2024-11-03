namespace Application.DTOs
{
    public class CourseDTO
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string Package { get; set; }
        public int Semester { get; set; }
        public string Level { get; set; }
        public List<Guid> ProfessorIds { get; set; } = new List<Guid>();
    }
}
