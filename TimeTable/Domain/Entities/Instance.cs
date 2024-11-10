using System.Text.Json;

namespace Domain.Entities
{
    public class Instance
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public List<Professor> Professors { get; set; } = new List<Professor>();
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Event> Events { get; set; } = new List<Event>();
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public HashSet<Constraint> Constraints { get; set; } = new HashSet<Constraint>();
        public List<Timeslot> TimeSlots { get; set; } = new List<Timeslot>();

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
            Professors = professors ?? new List<Professor>();
            Courses = courses ?? new List<Course>();
            Groups = groups ?? new List<Group>();
            Rooms = rooms ?? new List<Room>();
            Constraints = constraints ?? new HashSet<Constraint>();
            TimeSlots = timeSlots ?? new List<Timeslot>();
            Events = new List<Event>();
        }

        private void LoadFromJson(string configPath)
        {
            var jsonString = File.ReadAllText(configPath);
            var data = JsonSerializer.Deserialize<Instance>(jsonString);

            Courses = data?.Courses ?? new List<Course>();
            Groups = data?.Groups ?? new List<Group>();
            Rooms = data?.Rooms ?? new List<Room>();
            Constraints = data?.Constraints ?? new HashSet<Constraint>();
            Professors = data?.Professors ?? new List<Professor>();
            TimeSlots = data?.TimeSlots ?? new List<Timeslot>();
            Events = data?.Events ?? new List<Event>();
        }

        public void UploadToJson(string configPath)
        {
            var jsonString = JsonSerializer.Serialize(this, JsonSerializerOptions);
            File.WriteAllText(configPath, jsonString);
        }
    }
}