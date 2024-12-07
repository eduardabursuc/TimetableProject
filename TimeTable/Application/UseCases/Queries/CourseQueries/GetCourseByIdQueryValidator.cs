using FluentValidation;

namespace Application.UseCases.Queries.CourseQueries
{
    public abstract class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
    {
        protected GetCourseByIdQueryValidator()
        {
            RuleFor(t => t.CourseId).NotEmpty();
        }
    }
}