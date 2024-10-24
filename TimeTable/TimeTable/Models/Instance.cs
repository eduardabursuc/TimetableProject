using System.Runtime.InteropServices;

namespace TimeTable.Models;
using System.Text.Json;

public class Instance
{
    public List<Professor> Professors { get; set; }
    public List<Course> Courses { get; set; }
    public List<Group> Groups { get; set; }
    public List<Room> Rooms { get; set; }
    public List<Constraint> Constraints { get; set; }
    public List<Timeslot> TimeSlots { get; set; }

    public Instance(string config_path)
    {
        LoadFromJson(config_path);
    }

    private void LoadFromJson(string configPath)
    {
        var jsonString = File.ReadAllText(configPath);
        var configData = JsonSerializer.Deserialize<ConfigData>(jsonString);
        
        Professors = configData.Professors;
        
        Courses = new List<Course>();
        
        foreach (Professor professor in Professors)
            foreach (Course course in professor.Courses)
                if (!Courses.Contains(course))
                    Courses.Add(course);
        TimeSlots = new List<Timeslot>();
        Groups = configData.Groups;
        Rooms = configData.Rooms;
        foreach (var day in configData.Days)
            foreach (var hour in configData.Hours)
                TimeSlots.Add(new Timeslot(day, hour));
        
        Constraints = configData.Constraints;
        
    }

    public class ConfigData
    {
        public List<Professor> Professors { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Group> Groups { get; set; }
        public List<string> Days { get; set; }
        public List<string> Hours { get; set; }
        public List<Constraint> Constraints { get; set; }

        public ConfigData(List<Professor> professors, List<Room> rooms, List<Group> groups, List<string> days, List<string> hours, List<Constraint> constraints)
        {
            Professors = professors;
            Rooms = rooms;
            Groups = groups;
            Days = days;
            Hours = hours;
            Constraints = constraints;
        }
        
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
        
    }
}