using Application.DTOs;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public class CreateConstraintCommand : IRequest<Result<Guid>>
    {
        public ConstraintType Type { get; set; }
        public Guid? ProfessorId { get; set; }
        public string? CourseName { get; set; }
        public string? RoomName { get; set; }
        public string? WantedRoomName { get; set; }
        public string? GroupName { get; set; }
        public string? Day { get; set; }
        public string? Time { get; set; }
        public string? WantedDay { get; set; }
        public string? WantedTime { get; set; }
        public string? Event { get; set; }

        protected CreateConstraintCommand() { }

        public CreateConstraintCommand(ConstraintDto constraint)
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