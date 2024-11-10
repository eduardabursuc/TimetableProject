using FluentValidation;

namespace Application.UseCases.Queries
{
    public class GetCourseByNameQueryValidator : AbstractValidator<GetCourseByNameQuery>
    {
        public GetCourseByNameQueryValidator()
        {
            RuleFor(t => t.CourseName).NotEmpty();
        }
    }
}