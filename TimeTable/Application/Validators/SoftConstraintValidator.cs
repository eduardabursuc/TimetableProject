using Domain.Entities;
using Domain.Repositories;

namespace Application.Validators
{
    public class SoftConstraintValidator(ICourseRepository courseRepo)
    {

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