using Domain.Entities;
using Domain.Repositories;

namespace Application.Validators
{
    public class CoursesValidator(ICourseRepository repository)
    {
        public Tuple<bool, string> Validate(Course course)
        {
            if (string.IsNullOrEmpty(course.CourseName))
            {
                return Tuple.Create(false, "Course name is required.");
            }

            if (course.Credits <= 0)
            {
                return Tuple.Create(false, "Credits must be greater than zero.");
            }

            if (string.IsNullOrEmpty(course.Package))
            {
                return Tuple.Create(false, "Package is required.");
            }

            if (course.Semester <= 0)
            {
                return Tuple.Create(false, "Semester must be greater than zero.");
            }

            if (string.IsNullOrEmpty(course.Level))
            {
                return Tuple.Create(false, "Level is required.");
            }

            return !repository.GetByNameAsync(course.CourseName).Result.IsSuccess ? Tuple.Create(false, "Course name must be unique.") : Tuple.Create(true, "Course is valid.");
        }
    }
}