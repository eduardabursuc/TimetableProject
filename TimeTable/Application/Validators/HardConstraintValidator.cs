using Domain.Entities;

namespace Application.Validators;

public class HardConstraintValidator(Instance instance)
{
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
        if (!IsSameOrNestedGroup(event1.Group, event2.Group)) return true;

        var course1 = instance.Courses.FirstOrDefault(c => c.CourseName == event1.CourseName);
        var course2 = instance.Courses.FirstOrDefault(c => c.CourseName == event2.CourseName);

        if (course1 == null || course2 == null) return true;

        // Apply group-specific validation logic
        return course1.Package != course2.Package || course1.Level != course2.Level || course1.Semester != course2.Semester ||
               value1.Item2 != value2.Item2 || value1.Item1 != value2.Item1;
    }

    private static bool IsSameOrNestedGroup(string group1, string group2)
    {
        // Check if one group is a prefix of the other (e.g., 2E and 2E3, 2MISS and 2MISS1)
        return group1.StartsWith(group2) || group2.StartsWith(group1);
    }
}