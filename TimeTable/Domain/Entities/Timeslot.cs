namespace Domain.Entities
{
    public class Timeslot
    {
        public Guid Id { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public List<Constraint> Constraints { get; set; } = new List<Constraint>();
    }
}
