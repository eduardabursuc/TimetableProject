namespace TimeTable.Models;

public class Timeslot
{
    public string Day { get; set; }
    public string Time { get; set; }

    public Timeslot(string day, string time)
    {
        Day = day;
        Time = time;
    }
}