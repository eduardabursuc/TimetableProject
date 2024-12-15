using Domain.Entities;

namespace Infrastructure.JoinTables
{
    public class ProfessorTimetable
    {
        public Guid ProfessorId { get; set; }
        public Professor Professor { get; set; }

        public Guid TimetableId { get; set; }
        public Timetable Timetable { get; set; }
    }
}