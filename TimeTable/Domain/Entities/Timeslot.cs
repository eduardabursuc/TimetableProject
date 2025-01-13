
namespace Domain.Entities
{
    public class Timeslot(string day, string time, string? roomName)
    {
        public required string Day { get; set; } = day;
        public required string Time { get; set; } = time;
        
        public Timeslot(string day, string time) : this(day, time, null)
        {
        }

        public Timeslot() : this(string.Empty, string.Empty, null)
        {
        }
        
        private static int GetDayIndex(string day)
        {
            return day switch
            {
                "Monday" => 0,
                "Tuesday" => 1,
                "Wednesday" => 2,
                "Thursday" => 3,
                "Friday" => 4,
                "Saturday" => 5,
                "Sunday" => 6,
                _ => -1
            };
        }

        public bool isEarlier(Timeslot timeslot)
        {
            // Compare days first
            if (GetDayIndex(Day) < GetDayIndex(timeslot.Day)) return true;
            if (GetDayIndex(Day) > GetDayIndex(timeslot.Day)) return false;
            
            var t1EndTime = Time.Split('-')[1].Trim();
            var t2StartTime = timeslot.Time.Split('-')[0].Trim();

            // Convert to TimeSpan for comparison
            var t1End = TimeSpan.Parse(t1EndTime);
            var t2Start = TimeSpan.Parse(t2StartTime);

            // Compare times
            return t1End <= t2Start;
        }

        public bool inInterval(Timeslot timeslot)
        {
            // Compare days first
            if (GetDayIndex(Day) != GetDayIndex(timeslot.Day)) return false;

            var split = Time.Split('-');
            var t1StartTime = split[0];
            var t1EndTime = split[1];
            
            split = timeslot.Time.Split('-');
            var t2StartTime = split[0];
            var t2EndTime = split[1];

            // Convert to TimeSpan for comparison
            var t1Start = TimeSpan.Parse(t1StartTime);
            var t1End = TimeSpan.Parse(t1EndTime);
            var t2Start = TimeSpan.Parse(t2StartTime);
            var t2End = TimeSpan.Parse(t2EndTime);

            // Compare times
            return t2Start <= t1Start && t1End <= t2End;
        }

        public bool overlap(Timeslot timeslot)
        {
            // Compare days first
            if (GetDayIndex(Day) != GetDayIndex(timeslot.Day)) return false;

            var split = Time.Split('-');
            var t1StartTime = split[0];
            var t1EndTime = split[1];
            
            split = timeslot.Time.Split('-');
            var t2StartTime = split[0];
            var t2EndTime = split[1];

            // Convert to TimeSpan for comparison
            var t1Start = TimeSpan.Parse(t1StartTime);
            var t1End = TimeSpan.Parse(t1EndTime);
            var t2Start = TimeSpan.Parse(t2StartTime);
            var t2End = TimeSpan.Parse(t2EndTime);

            // Compare times
            return t1Start < t2End && t2Start < t1End;
        }

        public bool IsConsecutive(Timeslot timeslot)
        {
            // Compare days first
            if (GetDayIndex(Day) != GetDayIndex(timeslot.Day)) return false;
            
            var t1StartTime = Time.Split('-')[0].Trim();
            var t1EndTime = Time.Split('-')[1].Trim();
            
            var t2StartTime = timeslot.Time.Split('-')[0].Trim();
            var t2EndTime = timeslot.Time.Split('-')[1].Trim();

            // Convert to TimeSpan for comparison
            var t1Start = TimeSpan.Parse(t1StartTime);
            var t1End = TimeSpan.Parse(t1EndTime);
            var t2Start = TimeSpan.Parse(t2StartTime);
            var t2End = TimeSpan.Parse(t2EndTime);
            
            return t1EndTime == t2StartTime || t2EndTime == t1StartTime;
        }
        
    }
}