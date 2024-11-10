namespace Domain.Entities
{
    public class Course
    {
        public required string CourseName { get; set; }
        public required int Credits { get; set; }
        public required string Package { get; set; }
        public required int Semester { get; set; }
        public required string Level { get; set; }
    }
}
