namespace Domain.Entities
{
    public class ToDoItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDone { get; set; }

    }
}
