using FluentValidation;

namespace Application.UseCases.Queries
{
    public abstract class GetProfessorByIdQueryValidator : AbstractValidator<GetProfessorByIdQuery>
    {
        protected GetProfessorByIdQueryValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
        }
    }
}