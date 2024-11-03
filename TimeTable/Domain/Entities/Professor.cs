namespace Domain.Entities
{
    public class Professor
    {
        public string Id { get; set; }
        public string Name { get; set; }

        // Many-to-Many relationship with Course
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
