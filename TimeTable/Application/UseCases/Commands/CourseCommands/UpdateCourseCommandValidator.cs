using Application.Validators;
using AutoMapper;
using FluentValidation;

namespace Application.UseCases.Commands.CourseCommands
{
    public abstract class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        protected UpdateCourseCommandValidator(CoursesValidator validator, IMapper mapper)
        {
            RuleFor(t => t.Id).NotEmpty();
            Include(new CreateCourseCommandValidator(validator, mapper));
        }
    }
}