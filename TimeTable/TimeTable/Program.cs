using Application;
using Application.Validators;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories;

// Load the JSON data
const string jsonFilePath = "Configuration/config.json"; // Update this path to your JSON file location
var instance = new Instance(jsonFilePath);

var builder = WebApplication.CreateBuilder(args);

// Register services
RegisterServices(builder, instance);

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyHeader()                    
              .AllowAnyMethod();                   
    });
});

var app = builder.Build();

// Resolve ConstraintService and fetch constraints
await FetchConstraintsAsync(app, instance);

// Seed database at startup
await SeedDatabaseAtStartup(app, instance);

// Apply arc consistency and print results
ApplyArcConsistencyAndPrintResults(instance);

// Configure the HTTP request pipeline
ConfigureHttpPipeline(app);

await app.RunAsync();

instance.UploadToJson(jsonFilePath);
return;

void RegisterServices(WebApplicationBuilder builder, Instance instance)
{
    builder.Services.AddSingleton(instance);
    builder.Services.AddTransient<ConstraintsValidator>();
    builder.Services.AddTransient<ConstraintService>();
    builder.Services.AddTransient<IConstraintRepository, ConstraintRepository>();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

async Task FetchConstraintsAsync(WebApplication app, Instance instance)
{
    using var scope = app.Services.CreateScope();
    var constraintService = scope.ServiceProvider.GetRequiredService<ConstraintService>();
    instance.Constraints = await constraintService.GetConstraintsAsync();
}

async Task SeedDatabaseAtStartup(WebApplication app, Instance instance)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await ApplyMigrationsAsync(dbContext);
    await SeedDatabaseAsync(dbContext, instance);
}

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

    app.MapControllers();
}

async Task ApplyMigrationsAsync(ApplicationDbContext dbContext)
{
    if (dbContext.Database.IsRelational())
    {
        await dbContext.Database.MigrateAsync();
    }
}

async Task SeedDatabaseAsync(ApplicationDbContext dbContext, Instance instance)
{
    await SeedEntitiesAsync(dbContext, instance.Professors, dbContext.Professors, p => p.Name);
    await SeedEntitiesAsync(dbContext, instance.Courses, dbContext.Courses, c => c.CourseName);
    await SeedEntitiesAsync(dbContext, instance.Groups, dbContext.Groups, g => g.Name);
    await SeedEntitiesAsync(dbContext, instance.Rooms, dbContext.Rooms, r => r.Name);
    await SeedEntitiesAsync(dbContext, instance.Constraints, dbContext.Constraints, c => new { c.Type, c.ProfessorId, c.CourseName, c.GroupName, c.Day, c.Time });
}

async Task SeedEntitiesAsync<TEntity, TKey>(ApplicationDbContext dbContext, IEnumerable<TEntity> entities, DbSet<TEntity> dbSet, Func<TEntity, TKey> keySelector) where TEntity : class
{
    foreach (var entity in entities)
    {
        var entityKey = keySelector(entity);
        if (EqualityComparer<TKey>.Default.Equals(entityKey, default(TKey)))
        {
            continue; // Skip entities with default keys
        }

        var existingEntity = dbSet.Local.FirstOrDefault(e => EqualityComparer<TKey>.Default.Equals(keySelector(e), entityKey));
        if (existingEntity != null)
        {
            dbContext.Entry(existingEntity).State = EntityState.Detached;
        }

        if (!dbSet.AsEnumerable().Any(e => EqualityComparer<TKey>.Default.Equals(keySelector(e), entityKey)))
        {
            dbSet.Add(entity);
        }
    }
    await dbContext.SaveChangesAsync();
}

void ApplyArcConsistencyAndPrintResults(Instance instance)
{
    var arcConsistency = new ArcConsistency(instance);
    if (arcConsistency.ApplyArcConsistencyAndBacktracking(out var solution))
    {
        Console.WriteLine("Arc consistency applied successfully. Solution found:");
        ArcConsistency.PrintSolution(solution, instance);
    }
    else
    {
        Console.WriteLine("No solution found.");
    }
}

public abstract partial class Program
{
    protected Program() { }
}