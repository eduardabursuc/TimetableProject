using Domain.Entities;
using Domain.Repositories;

namespace Application.Validators;

public class HardConstraintValidator(ICourseRepository courseRepo, IGroupRepository groupRepo)
{
    
    public bool ValidateNoOverlap((Room, Timeslot) value1, (Room, Timeslot) value2)
    {
        return value1.Item1 != value2.Item1 || value1.Item2 != value2.Item2;
    }

    
    public bool ValidateRoomCapacity(Room room, string eventName)
    {
        return eventName.ToLower() switch
        {
            "course" => room.Capacity > 90,
            "seminary" => room.Capacity > 30,
            "laboratory" => room.Capacity > 30,
            _ => true
        };
    }

    public bool ValidateGroupOverlap(Event event1, Event event2, (Room, Timeslot) value1, (Room, Timeslot) value2)
    {
        var group1 = groupRepo.GetByIdAsync(event1.GroupId).Result.Data;
        var group2 = groupRepo.GetByIdAsync(event2.GroupId).Result.Data;


        var course1 = courseRepo.GetByIdAsync(event1.CourseId).Result;
        var course2 = courseRepo.GetByIdAsync(event2.CourseId).Result;

        if (!course1.IsSuccess || !course2.IsSuccess) return true;

        if (TimeslotsOverlap(value1.Item2, event1.Duration, value2.Item2, event2.Duration))
        {
            if (IsSameOrNestedGroup(group1.Name, group2.Name)) return false;
            if (course1.Data.Package == "compulsory" || course2.Data.Package == "compulsory") return false; //sa nu se suprapuna un eveniment compulsory al unei grupe cu oricare altul al aceleiasi grupe
            if (course1.Data.Package == course2.Data.Package 
                && course1.Data.Level == course2.Data.Level 
                && course1.Data.Semester == course2.Data.Semester) 
                return false; //daca ambele evenimente sunt din acelasi pachet
        }
        return true;
    }
    public bool TimeslotsOverlap(Timeslot timeslot1, int duration1, Timeslot timeslot2, int duration2)
    {
        var startTime1 = TimeSpan.Parse(timeslot1.Time.Split(" - ")[0]);
        var endTime1 = startTime1.Add(TimeSpan.FromHours(duration1));

        var startTime2 = TimeSpan.Parse(timeslot2.Time.Split(" - ")[0]);
        var endTime2 = startTime2.Add(TimeSpan.FromHours(duration2));

        return startTime1 < endTime2 && startTime2 < endTime1;
    }

    private static bool IsSameOrNestedGroup(string group1, string group2)
    {
        // Check if one group is a prefix of the other (e.g., 2E and 2E3, 2MISS and 2MISS1)
        return group1.StartsWith(group2) || group2.StartsWith(group1);
    }
}