using Application.DTOs;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetToDoItemsQuery : IRequest<List<ToDoItemDTO>>
    {
    }
}
