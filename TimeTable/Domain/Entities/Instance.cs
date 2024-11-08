using System.Text.Json;

namespace Domain.Entities
{
    public class Instance
    {
        public List<Professor> Professors { get; set; }
        public List<Course> Courses { get; set; }
        public List<Event> Events { get; set; }
        public List<Group> Groups { get; set; }
        public List<Room> Rooms { get; set; }
        public HashSet<Constraint> Constraints { get; set; }
        public List<Timeslot> TimeSlots { get; set; }

        public Instance() { }

        public Instance(string configPath)
        {
            LoadFromJson(configPath);
        }

        public Instance(
            List<Professor> professors,
            List<Course> courses,
            List<Group> groups,
            List<Room> rooms,
            HashSet<Constraint> constraints,
            List<Timeslot> timeSlots)
        {
            Professors = professors;
            Courses = courses;
            Groups = groups;
            Rooms = rooms;
            Constraints = constraints;
            TimeSlots = timeSlots;
        }

        private void LoadFromJson(string configPath)
        {
            var jsonString = File.ReadAllText(configPath);
            var data = JsonSerializer.Deserialize<Instance>(jsonString);

            Courses = data.Courses;
            Groups = data.Groups;
            Rooms = data.Rooms;
            Constraints = data.Constraints;
            Professors = data.Professors;
            TimeSlots = data.TimeSlots;
            Events = data.Events;
            
        }

        public void UploadToJson(string configPath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            var jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(configPath, jsonString);
        }
    }
    
}