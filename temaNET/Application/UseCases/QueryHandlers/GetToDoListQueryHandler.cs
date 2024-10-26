using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetToDoItemsQueryHandler : IRequestHandler<GetToDoItemsQuery, List<ToDoItemDTO>>
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public GetToDoItemsQueryHandler(IToDoItemRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<ToDoItemDTO>> Handle(GetToDoItemsQuery request, CancellationToken cancellationToken)
        {
            var todoItems = await repository.GetAllAsync();
            return mapper.Map<List<ToDoItemDTO>>(todoItems) ?? new List<ToDoItemDTO>();
        }
    }
}
