using FluentValidation;
using AutoMapper;
using Application.Validators;
using Domain.Entities;

namespace Application.UseCases.Commands
{
    public class UpdateProfessorCommandValidator : AbstractValidator<UpdateProfessorCommand>
    {
        public UpdateProfessorCommandValidator(ProfessorsValidator validator, IMapper mapper, Instance instance)
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateProfessorCommandValidator(validator, mapper, instance));
        }
    }
}