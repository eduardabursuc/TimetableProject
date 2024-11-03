using Application;
using Infrastructure;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

// Load the JSON data
var jsonFilePath = "Configuration/config.json"; // Update this path to your JSON file location
var instance = new Instance(jsonFilePath);

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed database at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Apply any pending migrations
    dbContext.Database.Migrate();

    // Seed Professors if not already present
    if (!dbContext.Professors.Any())
    {
        dbContext.Professors.AddRange(instance.Professors);
        dbContext.SaveChanges();
    }

    // Seed unique Courses
    foreach (var course in instance.Courses)
    {
        if (!dbContext.Courses.Any(c => c.CourseName == course.CourseName))
        {
            dbContext.Courses.Add(course);
        }
    }
    dbContext.SaveChanges();

    // Seed Groups
    if (!dbContext.Groups.Any())
    {
        dbContext.Groups.AddRange(instance.Groups);
        dbContext.SaveChanges();
    }

    // Seed Rooms
    if (!dbContext.Rooms.Any())
    {
        dbContext.Rooms.AddRange(instance.Rooms);
        dbContext.SaveChanges();
    }

    // Seed Constraints
    if (!dbContext.Constraints.Any())
    {
        dbContext.Constraints.AddRange(instance.Constraints);
        dbContext.SaveChanges();
    }

    // Seed Timeslots
    if (!dbContext.Timeslots.Any())
    {
        dbContext.Timeslots.AddRange(instance.TimeSlots);
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
