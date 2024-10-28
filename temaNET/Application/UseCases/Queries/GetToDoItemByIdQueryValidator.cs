using FluentValidation;

namespace Application.UseCases.Queries
{
    public class GetToDoItemByIdQueryValidator : AbstractValidator<GetToDoItemByIdQuery>
    {
        public GetToDoItemByIdQueryValidator()
        {
            RuleFor(q => q.Id).NotEmpty();
        }
    }
}