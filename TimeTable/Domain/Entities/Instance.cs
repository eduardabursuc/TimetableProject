
namespace Domain.Entities
{
    public class Instance
    {
        
        public List<Event> Events { get; set; } = [];
        public List<Timeslot> Timeslots { get; set; } = [];

        public Instance() { }
        
    }
}