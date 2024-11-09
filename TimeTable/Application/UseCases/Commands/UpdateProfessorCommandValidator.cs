using FluentValidation;
using AutoMapper;
using Application.Validators;

namespace Application.UseCases.Commands
{
    public class UpdateProfessorCommandValidator : AbstractValidator<UpdateProfessorCommand>
    {
        public UpdateProfessorCommandValidator(ProfessorsValidator validator, IMapper mapper)
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateProfessorCommandValidator(validator, mapper));
        }
    }
}