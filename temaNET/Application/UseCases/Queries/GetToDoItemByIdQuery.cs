using Application.DTOs;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetToDoItemByIdQuery : IRequest<ToDoItemDTO>
    {
        public Guid Id { get; set; }
    }
}
