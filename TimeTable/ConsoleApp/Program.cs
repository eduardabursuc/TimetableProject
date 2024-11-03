

using System.Text.Json;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    public static async Task Main(string[] args)
    {
        // Set up dependency injection
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql("host=localhost;port=5432;database=TimeTable;userid=postgres; password=student"))
            .BuildServiceProvider();

        // Seed the database
        await SeedDatabase(serviceProvider);
    }

    private static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Check if the database is created, and if not, create it
        await context.Database.EnsureCreatedAsync();

        // Load the JSON data
        var jsonFilePath = "../../../config.json"; // Update this path to your JSON file location
        var jsonData = await File.ReadAllTextAsync(jsonFilePath);

        var seedData = JsonSerializer.Deserialize<SeedData>(jsonData);

        // Seed Professors
        foreach (var professor in seedData.Professors)
        {
            var courses = new List<Course>();
            foreach (var course in professor.Courses)
            {
                var newCourse = new Course
                {
                    Id = Guid.NewGuid(), // Generating a new ID for Course
                    CourseName = course.CourseName,
                    Credits = course.Credits,
                    Package = course.Package,
                    Semester = course.Semester,
                    Level = course.Level
                };
                courses.Add(newCourse);
                context.Courses.Add(newCourse); // Add Course to the context
            }

            var newProfessor = new Professor
            {
                Id = professor.Id,
                Name = professor.Name,
                Courses = courses
            };

            context.Professors.Add(newProfessor);
        }

        // Seed Courses (if needed)
        foreach (var course in seedData.Courses)
        {
            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                CourseName = course.CourseName,
                Credits = course.Credits,
                Package = course.Package,
                Semester = course.Semester,
                Level = course.Level
            };
            context.Courses.Add(newCourse);
        }

        // Seed Groups
        foreach (var group in seedData.Groups)
        {
            var newGroup = new Group
            {
                Id = Guid.NewGuid(),
                Name = group.Name,
                Type = string.IsNullOrEmpty(group.Type) ? "DefaultType" : group.Type // Assign default value if Type is null or empty
            };
            context.Groups.Add(newGroup);
        }

        // Seed Rooms
        foreach (var room in seedData.Rooms)
        {
            var newRoom = new Room
            {
                Id = Guid.NewGuid(),
                Name = room.Name ?? "DefaultRoomName", // Assign default value if Name is null
                Capacity = room.Capacity
            };
            context.Rooms.Add(newRoom);
        }


        var savedRecordsCount = await context.SaveChangesAsync();

        Console.WriteLine($"Seeding complete! {savedRecordsCount} records were added to the database.");
        Console.WriteLine($"Professors seeded: {seedData.Professors.Count}");
        Console.WriteLine($"Courses seeded: {seedData.Courses.Count}");
        Console.WriteLine($"Groups seeded: {seedData.Groups.Count}");
        Console.WriteLine($"Rooms seeded: {seedData.Rooms.Count}");
    }


    private class SeedData
    {
        public List<Professor> Professors { get; set; }
        public List<Course> Courses { get; set; }
        public List<Group> Groups { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
