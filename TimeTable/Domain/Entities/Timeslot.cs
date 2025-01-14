using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Timeslot(string day, string time)
    {
        public required string Day { get; set; } = day;
        public required string Time { get; set; } = time;
        

        public Timeslot() : this(string.Empty, string.Empty)
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
        
        private static (TimeSpan Start, TimeSpan End) ParseTimeslot(string timeRange)
        {
            var split = timeRange.Split('-');
            return (TimeSpan.Parse(split[0].Trim()), TimeSpan.Parse(split[1].Trim()));
        }

        public bool InInterval(Timeslot timeslot)
        {
            // Compare days first
            if (GetDayIndex(Day) != GetDayIndex(timeslot.Day)) return false;

            var (t1Start, t1End) = ParseTimeslot(Time);
            var (t2Start, t2End) = ParseTimeslot(timeslot.Time);

            // Compare times
            return t2Start <= t1Start && t1End <= t2End;
        }

        public bool Overlap(Timeslot timeslot)
        {
            // Compare days first
            if (GetDayIndex(Day) != GetDayIndex(timeslot.Day)) return false;

            var (t1Start, t1End) = ParseTimeslot(Time);
            var (t2Start, t2End) = ParseTimeslot(timeslot.Time);

            // Compare times
            return t1Start < t2End && t2Start < t1End;
        }

        public bool IsConsecutive(Timeslot timeslot)
        {
            // Compare days first
            if (GetDayIndex(Day) != GetDayIndex(timeslot.Day)) return false;

            var (t1Start, t1End) = ParseTimeslot(Time);
            var (t2Start, t2End) = ParseTimeslot(timeslot.Time);

            // Check for consecutive times
            return t1End == t2Start || t2End == t1Start;
        }

        
    }
}