using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.GroupCommands
{
    public class UpdateGroupCommand : CreateGroupCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}
