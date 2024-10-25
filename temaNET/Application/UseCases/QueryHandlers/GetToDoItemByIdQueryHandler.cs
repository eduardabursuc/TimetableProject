using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetToDoItemByIdQueryHandler : IRequestHandler<GetToDoItemByIdQuery, ToDoItemDTO>
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public GetToDoItemByIdQueryHandler(IToDoItemRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<ToDoItemDTO> Handle(GetToDoItemByIdQuery request, CancellationToken cancellationToken)
        {
            var todoItem = await repository.GetByIdAsync(request.Id);
            return mapper.Map<ToDoItemDTO>(todoItem);
        }
    }
}
