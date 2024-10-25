using Application.UseCases.Commands;
using MediatR;

public class UpdateToDoItemCommand : CreateToDoItemCommand, IRequest<Guid>
{
    public Guid Id { get; set; }
}
