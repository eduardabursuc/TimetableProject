using Application;
using Infrastructure;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Validators;

// Load the JSON data
var jsonFilePath = "Configuration/config.json"; // Update this path to your JSON file location
var instance = new Instance(jsonFilePath);

var builder = WebApplication.CreateBuilder(args);

// Register services
RegisterServices(builder, instance);

var app = builder.Build();

// Seed database at startup
await SeedDatabaseAtStartup(app, instance);

// Configure the HTTP request pipeline
ConfigureHttpPipeline(app);

await app.RunAsync();

instance.UploadToJson(jsonFilePath);

void RegisterServices(WebApplicationBuilder builder, Instance instance)
{
    builder.Services.AddSingleton(instance);
    builder.Services.AddTransient<ConstraintsValidator>();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

async Task SeedDatabaseAtStartup(WebApplication app, Instance instance)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await ApplyMigrationsAsync(dbContext);
        await SeedDatabaseAsync(dbContext, instance);
    }
}

void ConfigureHttpPipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
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
    await SeedEntitiesAsync(dbContext, instance.Constraints, dbContext.Constraints, c => c.Type);
    await SeedEntitiesAsync(dbContext, instance.TimeSlots, dbContext.Timeslots, t => new { t.Day, t.Time });
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

public partial class Program
{
    protected Program(){}
}