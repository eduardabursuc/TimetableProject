using Application;
using Infrastructure;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Validators;
using System.Text.Json;

// Load the JSON data
var jsonFilePath = "Configuration/config.json"; // Update this path to your JSON file location
var instance = new Instance(jsonFilePath);

var builder = WebApplication.CreateBuilder(args);

// Register the instance as a singleton
builder.Services.AddSingleton(instance);

// Register the SoftConstraintsValidator
builder.Services.AddTransient<ConstraintsValidator>();

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
    foreach (var prof in instance.Professors)
    {
        if (!dbContext.Professors.Any(p => p.Name == prof.Name))
        {
            dbContext.Professors.Add(prof);
        }
    }
    dbContext.SaveChanges();

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
    foreach (var group in instance.Groups)
    {
        var existingGroup = dbContext.Groups.Local
            .FirstOrDefault(g => g.Name == group.Name);

        if (existingGroup != null)
        {
            dbContext.Entry(existingGroup).State = EntityState.Detached;
        }

        if (!dbContext.Groups.Any(g => g.Name == group.Name))
        {
            dbContext.Groups.Add(group);
        }
    }
    dbContext.SaveChanges();

    // Seed Rooms
    foreach (var room in instance.Rooms)
    {
        if (!dbContext.Rooms.Any(r => r.Name == room.Name))
        {
            dbContext.Rooms.Add(room);
        }
    }

    // Seed Constraints
    foreach (var constraint in instance.Constraints)
    {
        if (!dbContext.Constraints.Any(c => c.Type == constraint.Type))
        {
            dbContext.Constraints.Add(constraint);
        }
    }

    // Seed Timeslots
    foreach (var timeslot in instance.TimeSlots)
    {
        if (!dbContext.Timeslots.Any(t => t.Day == timeslot.Day && t.Time == timeslot.Time))
        {
            dbContext.Timeslots.Add(timeslot);
        }
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

instance.UploadToJson(jsonFilePath);
