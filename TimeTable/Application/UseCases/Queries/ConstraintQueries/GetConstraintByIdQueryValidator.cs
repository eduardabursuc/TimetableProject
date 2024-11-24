using FluentValidation;

namespace Application.UseCases.Queries.ConstraintQueries
{
    public abstract class GetConstraintByIdQueryValidator : AbstractValidator<GetConstraintByIdQuery>
    {
        protected GetConstraintByIdQueryValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
        }
    }
}