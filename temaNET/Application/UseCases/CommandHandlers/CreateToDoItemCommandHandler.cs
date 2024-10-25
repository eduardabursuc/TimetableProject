using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class CreateToDoItemCommandHandler : IRequestHandler<CreateToDoItemCommand, Guid>
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public CreateToDoItemCommandHandler(IToDoItemRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(CreateToDoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = mapper.Map<ToDoItem>(request);
            return await repository.AddAsync(todoItem);
        }
    }
}