using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class UpdateToDoItemCommandHandler : IRequestHandler<UpdateToDoItemCommand, Guid>
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public UpdateToDoItemCommandHandler(IToDoItemRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(UpdateToDoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = mapper.Map<ToDoItem>(request);
            await repository.UpdateAsync(todoItem);
            return todoItem.Id;
        }
    }
}