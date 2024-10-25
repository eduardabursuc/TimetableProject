using System.ComponentModel;

namespace Application.DTOs
{
    public class ToDoItemDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDone { get; set; }
    }
}
