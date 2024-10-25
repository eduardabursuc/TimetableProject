using MediatR;

namespace Application.UseCases.Commands
{
    public class CreateToDoItemCommand : IRequest<Guid>
    {
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDone { get; set; } = false; //false by default
    }
}
