using Application.UseCases.Commands;
using Application.UseCases.Queries;
using Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblyContaining<CreateToDoItemCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<DeleteToDoItemByIdCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateToDoItemCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<GetToDoItemByIdQueryValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
