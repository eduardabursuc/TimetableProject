using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands;

public class CreateProfessorCommandValidator : AbstractValidator<CreateProfessorCommand>
{
    private readonly IMapper mapper;

    public CreateProfessorCommandValidator(ProfessorsValidator validator, IMapper mapper)
    {
        this.mapper = mapper;
        
        RuleFor(c => c)
            .Must(c =>
            {
                var result = validator.Validate(mapper.Map<Professor>(c));
                return result.Item1;
            })
            .WithMessage(c =>
            {
                var result = validator.Validate(mapper.Map<Professor>(c));
                return result.Item2;
            });
    }
}