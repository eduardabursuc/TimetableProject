using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string AccountType { get; init; }
    
    [NotMapped]
    public List<Timetable> Timetables { get; set; } = [];
    [NotMapped]
    public List<Professor> Professors { get; set; } = [];
    [NotMapped]
    public List<Room> Rooms { get; set; } = [];
    [NotMapped]
    public List<Course> Courses { get; set; } = [];
    [NotMapped]
    public List<Group> Groups { get; set; } = [];
}