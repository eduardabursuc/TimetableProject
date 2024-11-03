namespace Domain.Entities
{
    public class Course
    {
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string Package { get; set; }
        public int Semester { get; set; }
        public string Level { get; set; }

        // Many-to-Many relationship with Professor
        public List<Professor> Professors { get; set; } = new List<Professor>();
    }
}
