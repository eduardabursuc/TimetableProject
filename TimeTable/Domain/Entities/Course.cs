namespace Domain.Entities
{
    public class Course
    {
        public required string UserEmail { get; init; }
        public required string CourseName { get; init; }
        public required int Credits { get; init; }
        public required string Package { get; init; }
        public required int Semester { get; init; }
        public required string Level { get; init; }
    }
}