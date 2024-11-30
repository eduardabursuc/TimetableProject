using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.TimetableCommands
{
    public class UpdateTimetableCommand : IRequest<Result<Guid>>
    {
    }
}

