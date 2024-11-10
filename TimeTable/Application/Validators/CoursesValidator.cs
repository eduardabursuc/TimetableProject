using Domain.Entities;

namespace Application.Validators
{
    public class CoursesValidator
    {
        private Instance instance;

        public CoursesValidator(Instance instance)
        {
            this.instance = instance;
        }

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

            return Tuple.Create(true, "Course is valid.");
        }
    }
}