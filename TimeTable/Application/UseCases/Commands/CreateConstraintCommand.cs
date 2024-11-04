using Domain.Entities;
using MediatR;
using Application.DTOs;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommand : IRequest<Guid>
    {
        public ConstraintType Type { get; set; }
        public string? ProfessorId { get; set; }
        public string? CourseName { get; set; }
        public string? RoomName { get; set; }
        public string? WantedRoomName { get; set; }
        public string? GroupName { get; set; }
        public string? Day { get; set; }
        public string? Time { get; set; }
        public string? WantedDay { get; set; }
        public string? WantedTime { get; set; }
        public string? Event { get; set; }
        
        public CreateConstraintCommand() { }

        public CreateConstraintCommand(ConstraintDTO constraint)
        {
            Type = constraint.Type;
            ProfessorId = constraint.ProfessorId;
            CourseName = constraint.CourseName;
            RoomName = constraint.RoomName;
            WantedRoomName = constraint.WantedRoomName;
            GroupName = constraint.GroupName;
            Day = constraint.Day;
            Time = constraint.Time;
            WantedDay = constraint.WantedDay;
            WantedTime = constraint.WantedTime;
            Event = constraint.Event;
        }
    }
}
