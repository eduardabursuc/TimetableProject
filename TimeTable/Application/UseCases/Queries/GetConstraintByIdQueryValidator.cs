using FluentValidation;

namespace Application.UseCases.Queries
{
    public class GetConstraintByIdQueryValidator : AbstractValidator<GetConstraintByIdQuery>
    {
        public GetConstraintByIdQueryValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
        }
    }
}