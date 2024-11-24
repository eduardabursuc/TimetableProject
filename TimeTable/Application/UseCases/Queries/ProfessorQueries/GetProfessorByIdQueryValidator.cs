using FluentValidation;

namespace Application.UseCases.Queries.ProfessorQueries
{
    public abstract class GetProfessorByIdQueryValidator : AbstractValidator<GetProfessorByIdQuery>
    {
        protected GetProfessorByIdQueryValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
        }
    }
}