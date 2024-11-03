using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommand : IRequest<Guid>
    {
        public ConstraintType Type { get; set; }
        public Guid ProfessorId { get; set; }
        public Guid CourseId { get; set; }
        public Guid RoomId { get; set; }
        public Guid? WantedRoomId { get; set; }
        public Guid GroupId { get; set; }
        public Guid? WantedTimeslotId { get; set; }
        public string Event { get; set; }
        public List<TimeslotDTO> Timeslots { get; set; } = new List<TimeslotDTO>();

        public CreateConstraintCommand() { }

        public CreateConstraintCommand(ConstraintDTO constraint)
        {
            Type = constraint.Type;
            ProfessorId = constraint.ProfessorId;
            CourseId = constraint.CourseId;
            RoomId = constraint.RoomId;
            WantedRoomId = constraint.WantedRoomId;
            GroupId = constraint.GroupId;
            WantedTimeslotId = constraint.WantedTimeslotId;
            Event = constraint.Event;
            Timeslots = constraint.Timeslots;
        }
    }
}
