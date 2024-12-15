using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ProfessorCommands
{
    public class AddTimetableToProfessorCommand : IRequest<Result<Unit>>
    {
        public required Guid Id { get; init; }
        public required Guid TimetableId { get; init; }
    
        public AddTimetableToProfessorCommand() { }
            
    }
}

