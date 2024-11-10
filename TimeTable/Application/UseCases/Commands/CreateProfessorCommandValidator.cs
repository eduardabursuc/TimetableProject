using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands
{
    public class CreateProfessorCommandValidator : AbstractValidator<CreateProfessorCommand>
    {
        public CreateProfessorCommandValidator(ProfessorsValidator validator, IMapper mapper, Instance instance)
        {
            RuleFor(c => c)
                .Must(c =>
                {
                    var professor = mapper.Map<Professor>(c);
                    var result = validator.Validate(professor);
                    return result.Item1;
                })
                .WithMessage(c =>
                {
                    var professor = mapper.Map<Professor>(c);
                    var result = validator.Validate(professor);
                    return result.Item2;
                });
        }
    }
}