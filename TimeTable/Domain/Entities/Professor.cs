namespace Domain.Entities
{
    public class Professor
    {
<<<<<<< Updated upstream
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
=======
        public Guid Id { get; set; }
        public required string Name { get; set; }
>>>>>>> Stashed changes

        // Many-to-Many relationship with Course
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
