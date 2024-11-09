using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    private readonly IMapper mapper;

    public CreateCourseCommandValidator(CoursesValidator validator, IMapper mapper)
    {
        this.mapper = mapper;
        
        RuleFor(c => c)
            .Must(c =>
            {
                var result = validator.Validate(mapper.Map<Course>(c));
                return result.Item1;
            })
            .WithMessage(c =>
            {
                var result = validator.Validate(mapper.Map<Course>(c));
                return result.Item2;
            });
    }
}