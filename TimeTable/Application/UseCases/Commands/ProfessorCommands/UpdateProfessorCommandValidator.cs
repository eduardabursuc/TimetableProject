using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public abstract class UpdateProfessorCommandValidator : AbstractValidator<UpdateProfessorCommand>
    {
        protected UpdateProfessorCommandValidator(ProfessorsValidator validator, IMapper mapper, Instance instance)
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateProfessorCommandValidator(validator, mapper, instance));
        }
    }
}