using FluentValidation;

namespace Application.UseCases.Queries
{
    public abstract class GetConstraintByIdQueryValidator : AbstractValidator<GetConstraintByIdQuery>
    {
        protected GetConstraintByIdQueryValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
        }
    }
}