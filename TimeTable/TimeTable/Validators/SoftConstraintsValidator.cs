namespace TimeTable.Validators;
using Models;

public class SoftConstraintsValidator
{
    private Instance _instance;
    
    public SoftConstraintsValidator(Instance instance)
    {
        _instance = instance;
    }
    
    public bool Validate(Constraint constraint)
    {
        switch (constraint.Type)
        {
            case ConstraintType.ROOM_CHANGE:
                return RoomChange(constraint);
            case ConstraintType.TIME_CHANGE:
                return TimeChange(constraint);
            case ConstraintType.DAY_CHANGE:
                return DayChange(constraint);
            case ConstraintType.INTERVAL_AVAILABILITY:
                return IntervalAvailability(constraint);
            case ConstraintType.INTERVAL_UNAVAILABILIY:
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

        return false;
    }
    
    private bool RoomChange(Constraint constraint)
    {
        return constraint.Room != null &&
                (
                    (constraint.Course != null && constraint.Timeslots != null) ||
                    (constraint.Professor != null && constraint.Timeslots != null) ||
                    (constraint.Group != null && constraint.Timeslots != null) ||
                    (constraint.Group != null && constraint.Event != null && constraint.Course != null)
                ) ;
    }
    
    private bool TimeChange(Constraint constraint)
    {
        return constraint.Timeslots.Count > 0;
    }
    
    private bool DayChange(Constraint constraint)
    {
        return 
            constraint.Timeslots.Count != null &&
            (
                (constraint.Course != null && constraint.Event != null) || 
                false // TODO: Add the rest of the conditions
            );
    }
    
    private bool IntervalAvailability(Constraint constraint)
    {
        return constraint.Timeslots != null && constraint.Professor != null; 
    }
    
    private bool IntervalUnavailability(Constraint constraint)
    {
        return constraint.Timeslots != null && constraint.Professor != null;
    }
    
    private bool WeekEvenness(Constraint constraint)
    {
        return constraint.Course != null;
    }
    
    private bool AddWindow(Constraint constraint)
    {
        return constraint.Timeslots != null && constraint.Professor != null;
    }
    
    private bool RemoveWindow(Constraint constraint)
    {
        return constraint.Timeslots != null && constraint.Professor != null;
    }
    
    private bool DayOff(Constraint constraint)
    {
        return constraint.Timeslots != null && constraint.Professor != null;
    }
    
    private bool ConsecutiveHours(Constraint constraint)
    {
        return constraint.Timeslots != null && constraint.Professor != null;
    }
    
    private bool LectureBeforeLabs(Constraint constraint)
    {
        return constraint.Course != null || constraint.Professor != null;
    }
    
    
    
    
}