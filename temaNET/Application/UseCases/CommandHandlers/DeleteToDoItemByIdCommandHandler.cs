using Application.UseCases.Commands;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class DeleteToDoItemByIdCommandHandler : IRequestHandler<DeleteToDoItemByIdCommand>
    {
        private readonly IToDoItemRepository repository;

        public DeleteToDoItemByIdCommandHandler(IToDoItemRepository repository)
        {
            this.repository = repository;
        }

        public async Task Handle(DeleteToDoItemByIdCommand request, CancellationToken cancellationToken)
        {
            await repository.DeleteAsync(request.Id);
        }
    }
}
