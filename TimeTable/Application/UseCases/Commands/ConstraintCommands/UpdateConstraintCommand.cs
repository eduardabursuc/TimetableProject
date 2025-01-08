using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public class UpdateConstraintCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
        public Guid TimetableId { get; set; }
        public ConstraintType Type { get; set; }
        public Guid? ProfessorId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? WantedRoomId { get; set; }
        public Guid? GroupId { get; set; }
        public string? Day { get; set; }
        public string? Time { get; set; }
        public string? WantedDay { get; set; }
        public string? WantedTime { get; set; }
        public string? Event { get; set; }
        public string ProfessorEmail { get; set; }
    }
}