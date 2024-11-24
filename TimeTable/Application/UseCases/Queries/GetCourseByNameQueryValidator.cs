using FluentValidation;

namespace Application.UseCases.Queries
{
    public abstract class GetCourseByNameQueryValidator : AbstractValidator<GetCourseByNameQuery>
    {
        protected GetCourseByNameQueryValidator()
        {
            RuleFor(t => t.CourseName).NotEmpty();
        }
    }
}