namespace Application.Validators;
using Domain.Entities;
using System.Linq;

public class SoftConstraintsValidator
{
    private Instance instance;

    public SoftConstraintsValidator(Instance instance)
    {
        this.instance = instance;
    }

    public Tuple<bool, string> Validate(Constraint constraint)
    {
        switch (constraint.Type)
        {
            case ConstraintType.ROOM_CHANGE:
                return RoomChange(constraint);
            case ConstraintType.ROOM_PREFERENCE:
                return RoomPreference(constraint);
            case ConstraintType.TIME_CHANGE:
                return TimeChange(constraint);
            case ConstraintType.DAY_CHANGE:
                return DayChange(constraint);
            case ConstraintType.INTERVAL_AVAILABILITY:
                return IntervalAvailability(constraint);
            case ConstraintType.INTERVAL_UNAVAILABILITY:
                return IntervalUnavailability(constraint);
            case ConstraintType.WEEK_EVENNESS:
                return WeekEvenness(constraint);
            case ConstraintType.ADD_WINDOW:
                return AddWindow(constraint);
            case ConstraintType.REMOVE_WINDOW:
                return RemoveWindow(constraint);
            case ConstraintType.DAY_OFF:
                return DayOff(constraint);
            case ConstraintType.CONSECUTIVE_HOURS:
                return ConsecutiveHours(constraint);
            case ConstraintType.LECTURE_BEFORE_LABS:
                return LectureBeforeLabs(constraint);
        }

        return Tuple.Create(false, "Constraint type is not recognized.");
    }

    private Tuple<bool, string> RoomChange(Constraint constraint)
    {
        var roomExists = constraint.RoomName != null && instance.Rooms.Any(r => r.Name == constraint.RoomName);
        
        if (!roomExists) return Tuple.Create(false, "Room is not specified.");
        
        var courseExists = constraint.CourseName != null && instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == "lecture" || constraint.Event == "laboratory" || constraint.Event == "seminary");
        var groupExists = constraint.GroupName != null && instance.Groups.Any(g => g.Name == constraint.GroupName);
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Any(t => t.Time == constraint.Time);
        // TODO: Add more cases for room change constraint type
        if ((courseExists && eventExists && groupExists) || (dayExists && timeExists))
            return Tuple.Create(true, "Room change constraint is valid.");
        return Tuple.Create(false, "A professor, group, timeslot or course should be specified.");
    }

    private Tuple<bool, string> RoomPreference(Constraint constraint)
    {
        var wantedRoomExists = constraint.WantedRoomName != null && instance.Rooms.Any(r => r.Name == constraint.WantedRoomName);
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var courseExists = constraint.CourseName != null && instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == "lecture" || constraint.Event == "laboratory" || constraint.Event == "seminary");
        
        if (!wantedRoomExists)
        {
            return Tuple.Create(false, "Wanted room is not specified or does not exists.");
        }
        if(professorExists || (courseExists && eventExists))
        {
            return Tuple.Create(true, "Room preference constraint is valid.");
        }
        return Tuple.Create(true, "A professor or a course and the event should be specified.");
    }

    private Tuple<bool, string> TimeChange(Constraint constraint)
    {
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Any(t => t.Time == constraint.Time);
        var courseExists = constraint.CourseName != null && instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == "lecture" || constraint.Event == "laboratory" || constraint.Event == "seminary");
        var groupExists = constraint.GroupName != null && instance.Groups.Any(g => g.Name == constraint.GroupName);
        
        if ((dayExists && timeExists) || (courseExists && eventExists && groupExists))
            return Tuple.Create(true, "Time change constraint is valid.");
        return Tuple.Create(false, "Timeslot is not specified, neither a course, event or group.");
    }

    private Tuple<bool, string> DayChange(Constraint constraint)
    {
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Any(t => t.Time == constraint.Time);
        var courseExists = constraint.CourseName != null && instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        var eventExists = constraint.Event != null && (constraint.Event == "lecture" || constraint.Event == "laboratory" || constraint.Event == "seminary");
        var groupExists = constraint.GroupName != null && instance.Groups.Any(g => g.Name == constraint.GroupName);
        
        if ((dayExists && timeExists) || (courseExists && eventExists && groupExists))
            return Tuple.Create(true, "Day change constraint is valid.");
        return Tuple.Create(false, "Timeslot is not specified, neither a course, event or group.");
    }

    private Tuple<bool, string> IntervalAvailability(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Any(t => t.Time == constraint.Time);
        
        if (dayExists && timeExists && professorExists )
        {
            return Tuple.Create(true, "Interval availability constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> IntervalUnavailability(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        var timeExists = constraint.Time != null && instance.TimeSlots.Any(t => t.Time == constraint.Time);
        
        if (dayExists && timeExists && professorExists)
        {
            return Tuple.Create(true, "Interval unavailability constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> WeekEvenness(Constraint constraint)
    {
        var courseExists = constraint.CourseName != null && instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        
        if (courseExists)
        {
            return Tuple.Create(true, "Week evenness constraint is valid.");
        }
        return Tuple.Create(false, "Course is not specified or does not exist.");
    }

    private Tuple<bool, string> AddWindow(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var timeExists = constraint.Time != null && instance.TimeSlots.Any(t => t.Time == constraint.Time);
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);

        if ((dayExists || timeExists) && professorExists)
        {
            return Tuple.Create(true, "Add window constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> RemoveWindow(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var timeExists = constraint.Time != null && instance.TimeSlots.Any(t => t.Time == constraint.Time);
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        
        if ((dayExists || timeExists) && professorExists)
        {
            return Tuple.Create(true, "Remove window constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> DayOff(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        
        if (dayExists && professorExists)
        {
            return Tuple.Create(true, "Day off constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> ConsecutiveHours(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var dayExists = constraint.Day != null && instance.TimeSlots.Any(t => t.Day == constraint.Day);
        
        if (dayExists && professorExists)
        {
            return Tuple.Create(true, "Consecutive hours constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> LectureBeforeLabs(Constraint constraint)
    {
        var professorExists = constraint.ProfessorId != null && instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        var courseExists = constraint.CourseName != null && instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        
        if (courseExists || professorExists)
        {
            return Tuple.Create(true, "Lecture before labs constraint is valid.");
        }
        return Tuple.Create(false, "Course or professor are not specified or do not exist.");
    }
}