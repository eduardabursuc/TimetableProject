using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class UpdateTimetableCommand : IRequest<Result<Guid>>
    {
        public required Timetable Timetable { get; set; }
    }
}

