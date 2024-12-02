// Application/ToDoItems/Queries/GetToDoItems/GetToDoItemsQuery.cs
using MediatR;

namespace Application.ToDoItems.Queries.GetToDoItems
{
    public class GetToDoItemsQuery : IRequest<Result<PaginatedList<ToDoItemDto>>>
    {
        public bool? IsDone { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}