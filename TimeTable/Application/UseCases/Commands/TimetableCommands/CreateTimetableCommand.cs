using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class CreateTimetableCommand : IRequest<Result<Timetable>>
    {
        public CreateTimetableCommand() { }
    }
}