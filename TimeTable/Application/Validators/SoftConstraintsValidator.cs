namespace Application.Validators;
using Domain.Entities;
using System.Linq;

public class SoftConstraintsValidator
{
    private Instance _instance;

    public SoftConstraintsValidator(Instance instance)
    {
        _instance = instance;
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
        if (constraint.RoomName == null) return Tuple.Create(false, "Room is not specified.");

        // Validate the room against the instance's room list
        bool roomExists = _instance.Rooms.Any(r => r.Name == constraint.RoomName);
        bool courseExists = constraint.CourseName != null && _instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        bool eventExists = constraint.Event != null && (constraint.Event == "lecture" || constraint.Event == "laboratory" || constraint.Event == "seminary");
        // TODO: Add more cases for room change constraint type
        if (roomExists && ( (courseExists && eventExists && courseExists) || constraint.Day != null))
            return Tuple.Create(true, "Room change constraint is valid.");
        else return Tuple.Create(false, "A professor, group, timeslot or course should be specified.");
    }

    private Tuple<bool, string> RoomPreference(Constraint constraint)
    {
        if (constraint.WantedRoomName == null) return Tuple.Create(false, "Wanted room is not specified.");

        // Validate the wanted room against the instance's room list
        bool roomExists = _instance.Rooms.Any(r => r.Name == constraint.WantedRoomName);

        if (roomExists)
            return Tuple.Create(true, "Room preference constraint is valid.");
        else return Tuple.Create(false, "Wanted room does not exist.");
    }

    private Tuple<bool, string> TimeChange(Constraint constraint)
    {
        if (constraint.Day != null && constraint.Time != null)
            return Tuple.Create(true, "Time change constraint is valid.");
        else return Tuple.Create(false, "Timeslots are not specified.");
    }

    private Tuple<bool, string> DayChange(Constraint constraint)
    {
        bool courseExists = constraint.CourseName != null && _instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        bool eventExists = constraint.Event != null && (constraint.Event == "lecture" || constraint.Event == "laboratory" || constraint.Event == "seminary");
        
        if( constraint.Day != null && constraint.Time != null && ((courseExists && eventExists) ))
            return Tuple.Create(true, "Day change constraint is valid.");
        else return Tuple.Create(false, "A professor, group, timeslot or course should be specified.");
    }

    private Tuple<bool, string> IntervalAvailability(Constraint constraint)
    {
        bool professorExists = constraint.ProfessorId != null && _instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        
        if (constraint.Day != null && professorExists )
        {
            return Tuple.Create(true, "Interval availability constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> IntervalUnavailability(Constraint constraint)
    {
        bool professorExists = constraint.ProfessorId != null && _instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        
        if (constraint.Day != null && professorExists)
        {
            return Tuple.Create(true, "Interval unavailability constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> WeekEvenness(Constraint constraint)
    {
        bool courseExists = constraint.CourseName != null && _instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        
        if (courseExists)
        {
            return Tuple.Create(true, "Week evenness constraint is valid.");
        }
        return Tuple.Create(false, "Course is not specified or does not exist.");
    }

    private Tuple<bool, string> AddWindow(Constraint constraint)
    {
        bool professorExists = constraint.ProfessorId != null && _instance.Professors.Any(p => p.Id == constraint.ProfessorId);

        if (constraint.Time != null && professorExists)
        {
            return Tuple.Create(true, "Add window constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> RemoveWindow(Constraint constraint)
    {
        bool professorExists = constraint.ProfessorId != null && _instance.Professors.Any(p => p.Id == constraint.ProfessorId);

        if (constraint.Time != null && professorExists)
        {
            return Tuple.Create(true, "Remove window constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> DayOff(Constraint constraint)
    {
        bool professorExists = constraint.ProfessorId != null && _instance.Professors.Any(p => p.Id == constraint.ProfessorId);

        if (constraint.Time != null && professorExists)
        {
            return Tuple.Create(true, "Day off constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> ConsecutiveHours(Constraint constraint)
    {
        bool professorExists = constraint.ProfessorId != null && _instance.Professors.Any(p => p.Id == constraint.ProfessorId);

        if (constraint.Time != null && professorExists)
        {
            return Tuple.Create(true, "Consecutive hours constraint is valid.");
        }
        return Tuple.Create(false, "Timeslots or professor are not specified.");
    }

    private Tuple<bool, string> LectureBeforeLabs(Constraint constraint)
    {
        bool professorExists = constraint.ProfessorId != null && _instance.Professors.Any(p => p.Id == constraint.ProfessorId);
        bool courseExists = constraint.CourseName != null && _instance.Courses.Any(c => c.CourseName == constraint.CourseName);
        
        if (courseExists || professorExists)
        {
            return Tuple.Create(true, "Lecture before labs constraint is valid.");
        }
        return Tuple.Create(false, "Course or professor are not specified or do not exist.");
    }
}