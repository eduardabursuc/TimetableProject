using Domain.Entities;

namespace Application.Validators
{
    public class SoftConstraintValidator(Instance instance)
    {
        private readonly Instance _instance = instance;

        public bool Validate(Constraint constraint, Event evnt, (Room, Timeslot) roomTimeTuple)
        {
            return constraint.Type switch
            {
                ConstraintType.SOFT_ROOM_PREFERENCE => ValidateRoomPreference(constraint, evnt, roomTimeTuple.Item1),
                ConstraintType.SOFT_DAY_OFF => ValidateDayOff(constraint, evnt, roomTimeTuple.Item2),
                ConstraintType.SOFT_WEEK_EVENNESS => ValidateWeekEvenness(constraint, evnt, roomTimeTuple.Item2),
                ConstraintType.SOFT_INTERVAL_AVAILABILITY => ValidateIntervalAvailability(constraint, evnt, roomTimeTuple.Item2),
                ConstraintType.SOFT_INTERVAL_UNAVAILABILITY => ValidateIntervalUnavailability(constraint, evnt, roomTimeTuple.Item2),
                _ => true
            };
        }
        
        public static bool ValidateLectureBeforeLabs(Constraint constraint, Event evnt1, Event evnt2, Timeslot ts1, Timeslot ts2)
        {
            if ( constraint.Type != ConstraintType.SOFT_LECTURE_BEFORE_LABS ) return true;
            if ( evnt1.CourseName != evnt2.CourseName ) return true;
            if ( evnt1.EventName == evnt2.EventName ) return true;
            if ( evnt1.EventName != "course" && evnt2.EventName != "course" ) return true;
            Timeslot courseTime;
            Timeslot labTime;
            if (evnt2.EventName == "course")
            {
                courseTime = ts2;
                labTime = ts1;
            }
            else
            {
                courseTime = ts1;
                labTime = ts2;
            }
            return courseTime.isEarlier(labTime);
        }
        
        public static bool ValidateConsecutiveHours(Constraint constraint, Event evnt1, Event evnt2, Timeslot ts1, Timeslot ts2)
        {
            if (constraint.Type != ConstraintType.SOFT_CONSECUTIVE_HOURS) return true;
            if (constraint.Day != ts1.Day) return true;
            if (constraint.ProfessorId == evnt1.ProfessorId && constraint.ProfessorId == evnt2.ProfessorId)
            {
                return ts1.IsConsecutive(ts2);
            }
            return true;
        }
    
        private static bool ValidateRoomPreference(Constraint constraint, Event evnt, Room room)
        {
            return constraint.ProfessorId != evnt.ProfessorId || constraint.WantedRoomId == room.Id;
        }
    
        private static bool ValidateDayOff(Constraint constraint, Event evnt, Timeslot timeslot)
        {
            return constraint.ProfessorId != evnt.ProfessorId || constraint.Day != timeslot.Day;
        }
    
        private static bool ValidateWeekEvenness(Constraint constraint, Event evnt, Timeslot timeslot)
        {
            return true;
        }
    
        private static bool ValidateIntervalAvailability(Constraint constraint, Event evnt, Timeslot timeslot)
        {
            if (constraint.ProfessorId != evnt.ProfessorId) return true;
            if (string.IsNullOrEmpty(constraint.Day) || string.IsNullOrEmpty(constraint.Time)) return true;
            var newTimeslot = new Timeslot
            {
                Day = constraint.Day,
                Time = constraint.Time
            };

            return timeslot.inInterval(newTimeslot);

        }
    
        private static bool ValidateIntervalUnavailability(Constraint constraint, Event evnt, Timeslot timeslot)
        {
            if (constraint.ProfessorId != evnt.ProfessorId) return true;
            if (string.IsNullOrEmpty(constraint.Day) || string.IsNullOrEmpty(constraint.Time)) return true;
            var newTimeslot = new Timeslot
            {
                Day = constraint.Day,
                Time = constraint.Time
            };

            return !timeslot.inInterval(newTimeslot) && !timeslot.overlap(newTimeslot);
        }

        

}

}