namespace Application.Validators;
using Domain.Entities;
using System.Globalization;

public class ConstraintsValidator(Instance instance)
{
    private const string Lecture = "lecture";
    private const string Laboratory = "laboratory";
    private const string Seminary = "seminary";
    private const string TimeslotsOrProfessorNotSpecified = "Timeslots or professor are not specified.";

    public Tuple<bool, string> Validate(Constraint constraint)
    {
        switch (constraint.Type)
        {
            case ConstraintType.HARD_NO_OVERLAP:
                return Tuple.Create(true, "No overlap constraint is valid.");
            case ConstraintType.HARD_YEAR_PRIORITY:
                return Tuple.Create(true, "Year priority constraint is valid.");
            case ConstraintType.HARD_ROOM_CAPACITY:
                return Tuple.Create(true, "Room capacity constraint is valid.");
            case ConstraintType.SOFT_ROOM_CHANGE:
                return RoomChange(constraint);
            case ConstraintType.SOFT_ROOM_PREFERENCE:
                return RoomPreference(constraint);
            case ConstraintType.SOFT_TIME_CHANGE:
                return TimeChange(constraint);
            case ConstraintType.SOFT_DAY_CHANGE:
                return DayChange(constraint);
            case ConstraintType.SOFT_INTERVAL_AVAILABILITY:
                return IntervalAvailability(constraint);
            case ConstraintType.SOFT_INTERVAL_UNAVAILABILITY:
                return IntervalUnavailability(constraint);
            case ConstraintType.SOFT_WEEK_EVENNESS:
                return WeekEvenness(constraint);
            case ConstraintType.SOFT_ADD_WINDOW:
                return AddWindow(constraint);
            case ConstraintType.SOFT_REMOVE_WINDOW:
                return RemoveWindow(constraint);
            case ConstraintType.SOFT_DAY_OFF:
                return DayOff(constraint);
            case ConstraintType.SOFT_CONSECUTIVE_HOURS:
                return ConsecutiveHours(constraint);
            case ConstraintType.SOFT_LECTURE_BEFORE_LABS:
                return LectureBeforeLabs(constraint);
        }

        return Tuple.Create(false, "Constraint type is not recognized.");
    }

    private Tuple<bool, string> RoomChange(Constraint constraint)
    {
        var courseExists = constraint.CourseName != null && instance.Courses.Exists(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == Lecture || constraint.Event == Laboratory || constraint.Event == Seminary);
        var groupExists = constraint.GroupName != null && instance.Groups.Exists(g => g.Name == constraint.GroupName);
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Exists(t => t.Time == constraint.Time);

        // Add more cases for room change constraint type
        if ((courseExists && eventExists && groupExists) || (dayExists && timeExists))
            return Tuple.Create(true, "Room change constraint is valid.");
        return Tuple.Create(false, "A professor, group, timeslot or course should be specified.");
    }

    private Tuple<bool, string> RoomPreference(Constraint constraint)
    {
        var wantedRoomExists = constraint.WantedRoomName != null && instance.Rooms.Exists(r => r.Name == constraint.WantedRoomName);
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var courseExists = constraint.CourseName != null && instance.Courses.Exists(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == Lecture || constraint.Event == Laboratory || constraint.Event == Seminary);

        if (!wantedRoomExists)
        {
            return Tuple.Create(false, "Wanted room is not specified or does not exists.");
        }
        if (professorExists || (courseExists && eventExists))
        {
            return Tuple.Create(true, "Room preference constraint is valid.");
        }
        return Tuple.Create(true, "A professor or a course and the event should be specified.");
    }

    private Tuple<bool, string> TimeChange(Constraint constraint)
    {
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Exists(t => t.Time == constraint.Time);
        var courseExists = constraint.CourseName != null && instance.Courses.Exists(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == Lecture || constraint.Event == Laboratory || constraint.Event == Seminary);
        var groupExists = constraint.GroupName != null && instance.Groups.Exists(g => g.Name == constraint.GroupName);

        if ((dayExists && timeExists) || (courseExists && eventExists && groupExists))
            return Tuple.Create(true, "Time change constraint is valid.");
        return Tuple.Create(false, "Timeslot is not specified, neither a course, event or group.");
    }

    private Tuple<bool, string> DayChange(Constraint constraint)
    {
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Exists(t => t.Time == constraint.Time);
        var courseExists = constraint.CourseName != null && instance.Courses.Exists(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == Lecture || constraint.Event == Laboratory || constraint.Event == Seminary);
        var groupExists = constraint.GroupName != null && instance.Groups.Exists(g => g.Name == constraint.GroupName);

        if ((dayExists && timeExists) || (courseExists && eventExists && groupExists))
            return Tuple.Create(true, "Day change constraint is valid.");
        return Tuple.Create(false, "Timeslot is not specified, neither a course, event or group.");
    }

    private Tuple<bool, string> IntervalAvailability(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && TimeRangeExists(constraint.Time);

        if (dayExists && timeExists && professorExists)
        {
            return Tuple.Create(true, "Interval availability constraint is valid.");
        }
        return Tuple.Create(false, TimeslotsOrProfessorNotSpecified);
    }

    private Tuple<bool, string> IntervalUnavailability(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && TimeRangeExists(constraint.Time);

        if (dayExists && timeExists && professorExists)
        {
            return Tuple.Create(true, "Interval unavailability constraint is valid.");
        }
        return Tuple.Create(false, TimeslotsOrProfessorNotSpecified);
    }

    private Tuple<bool, string> WeekEvenness(Constraint constraint)
    {
        var courseExists = constraint.CourseName != null && instance.Courses.Exists(c => c.CourseName == constraint.CourseName);

        if (courseExists)
        {
            return Tuple.Create(true, "Week evenness constraint is valid.");
        }
        return Tuple.Create(false, "Course is not specified or does not exist.");
    }

    private Tuple<bool, string> AddWindow(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var timeExists = constraint.Time != null && TimeRangeExists(constraint.Time);
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);

        if ((dayExists || timeExists) && professorExists)
        {
            return Tuple.Create(true, "Add window constraint is valid.");
        }
        return Tuple.Create(false, TimeslotsOrProfessorNotSpecified);
    }

    private Tuple<bool, string> RemoveWindow(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var timeExists = constraint.Time != null && instance.TimeSlots.Exists(t => t.Time == constraint.Time);
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);

        if ((dayExists || timeExists) && professorExists)
        {
            return Tuple.Create(true, "Remove window constraint is valid.");
        }
        return Tuple.Create(false, TimeslotsOrProfessorNotSpecified);
    }

    private Tuple<bool, string> DayOff(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);

        if (dayExists && professorExists)
        {
            return Tuple.Create(true, "Day off constraint is valid.");
        }
        return Tuple.Create(false, TimeslotsOrProfessorNotSpecified);
    }

    private Tuple<bool, string> ConsecutiveHours(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Exists(t => t.Day == constraint.Day);

        if (dayExists && professorExists)
        {
            return Tuple.Create(true, "Consecutive hours constraint is valid.");
        }
        return Tuple.Create(false, TimeslotsOrProfessorNotSpecified);
    }

    private Tuple<bool, string> LectureBeforeLabs(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Exists(p => p.Id == constraint.ProfessorId);
        var courseExists = constraint.CourseName != null && instance.Courses.Exists(c => c.CourseName == constraint.CourseName);

        if (courseExists || professorExists)
        {
            return Tuple.Create(true, "Lecture before labs constraint is valid.");
        }
        return Tuple.Create(false, "Course or professor are not specified or do not exist.");
    }

    private bool TimeRangeExists(string timeRange)
    {
        var times = timeRange.Split(" - ");
        var rangeStart = TimeSpan.Parse(times[0], CultureInfo.InvariantCulture);
        var rangeEnd = TimeSpan.Parse(times[1], CultureInfo.InvariantCulture);

        // Check if there is any time slot that overlaps with the given range
        foreach (var slot in instance.TimeSlots)
        {
            // Parse start and end times of the current slot
            var slotTimes = slot.Time.Split(" - ");
            var slotStart = TimeSpan.Parse(slotTimes[0], CultureInfo.InvariantCulture);
            var slotEnd = TimeSpan.Parse(slotTimes[1], CultureInfo.InvariantCulture);

            // Check if the time slot overlaps with the range
            var overlaps = slotStart < rangeEnd && slotEnd > rangeStart;
            if (overlaps)
            {
                return true; // Found an overlapping slot
            }
        }

        return false; // No overlapping slot found
    }
}