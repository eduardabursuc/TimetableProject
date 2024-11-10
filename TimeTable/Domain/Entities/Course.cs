namespace Domain.Entities
{
    public class Course
    {
<<<<<<< Updated upstream
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string Package { get; set; }
        public int Semester { get; set; }
        public string Level { get; set; }

        // Many-to-Many relationship with Professor
        public List<Professor> Professors { get; set; } = new List<Professor>();
=======
        public required string CourseName { get; set; }
        public required int Credits { get; set; }
        public required string Package { get; set; }
        public required int Semester { get; set; }
        public required string Level { get; set; }
>>>>>>> Stashed changes
    }
}
