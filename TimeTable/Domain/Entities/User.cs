using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string AccountType { get; init; }
    
    public List<Timetable> Timetables { get; set; } = [];
    public List<Professor> Professors { get; set; } = [];
    public List<Room> Rooms { get; set; } = [];
    public List<Course> Courses { get; set; } = [];
    public List<Group> Groups { get; set; } = [];
}