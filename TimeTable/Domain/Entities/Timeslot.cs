namespace Domain.Entities
{
    public class Timeslot
    {
        public string Day { get; set; }
        public string Time { get; set; }
        
        bool IsAvailable { get; set; }
        Event Event { get; set; }
        string RoomName { get; set; }
        
        public Timeslot(string day, string time)
        {
            Day = day;
            Time = time;
            IsAvailable = true;
        }
        
        public Timeslot(string day, string time, string roomName)
        {
            Day = day;
            Time = time;
            RoomName = roomName;
            IsAvailable = true;
        }
    }
}
