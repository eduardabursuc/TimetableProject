using Application;
using Application.Validators;
using Domain.Repositories;
using Infrastructure;
using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Create an instance for dependency injection (or get it dynamically if applicable)
var instance = new Instance(); // Replace with actual initialization if needed

// Register services
RegisterServices(builder, instance);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureHttpPipeline(app);

await app.RunAsync();

return;

// Method to register all required services
void RegisterServices(WebApplicationBuilder builder, Instance instance)
{
    builder.Services.AddSingleton(instance);
    builder.Services.AddTransient<ConstraintsValidator>();
    builder.Services.AddTransient<IConstraintRepository, ConstraintRepository>();
    builder.Services.AddTransient<ICourseRepository, CourseRepository>();
    builder.Services.AddTransient<IRoomRepository, RoomRepository>();
    builder.Services.AddTransient<IGroupRepository, GroupRepository>();
    builder.Services.AddTransient<IProfessorRepository, ProfessorRepository>();
    builder.Services.AddTransient<ITimetableRepository, TimetableRepository>();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddControllers(); // Registers services for controllers
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();
    
    // Register the CORS policy
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("*")  // Allow frontend origin
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}

// Method to configure the HTTP request pipeline
void ConfigureHttpPipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Enable the CORS policy
    app.UseCors("AllowAngularApp");
    
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers(); // Maps controller routes
}

public abstract partial class Program
{
    protected Program() { }
}