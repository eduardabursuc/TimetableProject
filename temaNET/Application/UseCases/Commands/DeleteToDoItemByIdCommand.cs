using MediatR;

namespace Application.UseCases.Commands
{
    public class DeleteToDoItemByIdCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
