using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Commands.CourseCommands
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator(CoursesValidator validator, IMapper mapper)
        {
            RuleFor(c => c)
                .Must(c =>
                {
                    var course = mapper.Map<Course>(c);
                    var result = validator.Validate(course);
                    return result.Item1;
                })
                .WithMessage(c =>
                {
                    var course = mapper.Map<Course>(c);
                    var result = validator.Validate(course);
                    return result.Item2;
                });
        }
    }
}