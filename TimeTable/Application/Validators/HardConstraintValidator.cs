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
        
        if (!IsSameOrNestedGroup(group1.Name, group2.Name)) return true;

        var course1 = courseRepo.GetByIdAsync(event1.CourseId).Result;
        var course2 = courseRepo.GetByIdAsync(event1.CourseId).Result;

        if (!course1.IsSuccess || !course2.IsSuccess) return true;

        // Apply group-specific validation logic
        return course1.Data.Package != course2.Data.Package || course1.Data.Level != course2.Data.Level || course1.Data.Semester != course2.Data.Semester ||
               value1.Item2 != value2.Item2 || value1.Item1 != value2.Item1;
    }

    private static bool IsSameOrNestedGroup(string group1, string group2)
    {
        // Check if one group is a prefix of the other (e.g., 2E and 2E3, 2MISS and 2MISS1)
        return group1.StartsWith(group2) || group2.StartsWith(group1);
    }
}