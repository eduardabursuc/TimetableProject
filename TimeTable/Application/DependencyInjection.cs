using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.UseCases.Commands.ConstraintCommands;
using Application.UseCases.Commands.CourseCommands;
using Application.UseCases.Commands.ProfessorCommands;
using Application.UseCases.Queries.ConstraintQueries;
using Application.UseCases.Queries.CourseQueries;
using Application.UseCases.Queries.ProfessorQueries;
using Application.Validators;
using Application.Utils;
using Domain.Entities;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblyContaining<CreateConstraintCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateConstraintCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<GetConstraintByIdQueryValidator>();

            // Course Validators
            services.AddValidatorsFromAssemblyContaining<CreateCourseCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateCourseCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<GetCourseByIdQueryValidator>();
            
            // Professor Validators
            services.AddValidatorsFromAssemblyContaining<CreateProfessorCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateProfessorCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProfessorByIdQueryValidator>();

            services.AddScoped<CoursesValidator>();
            services.AddScoped<ProfessorsValidator>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}