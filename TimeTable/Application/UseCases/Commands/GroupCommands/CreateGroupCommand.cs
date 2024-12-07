using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.GroupCommands
{
    public class CreateGroupCommand : IRequest<Result<Guid>>
    {
        public required string UserEmail { get; init; }
        public required string Name { get; init; }

        public CreateGroupCommand() { }
        
    }
}