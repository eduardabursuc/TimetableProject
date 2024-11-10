using FluentValidation;

namespace Application.UseCases.Queries
{
    public class GetProfessorByIdQueryValidator : AbstractValidator<GetProfessorByIdQuery>
    {
        public GetProfessorByIdQueryValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
        }
    }
}