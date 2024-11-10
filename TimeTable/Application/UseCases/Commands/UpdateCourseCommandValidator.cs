using FluentValidation;
using AutoMapper;
using Application.Validators;
using Domain.Entities;

namespace Application.UseCases.Commands
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator(CoursesValidator validator, IMapper mapper)
        {
            RuleFor(t => t.CourseName).NotEmpty();
            Include(new CreateCourseCommandValidator(validator, mapper));
        }
    }
}