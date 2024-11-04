using Application.UseCases.Commands;using Domain.Entities;
using MediatR;

public class UpdateConstraintCommand : CreateConstraintCommand, IRequest<Guid>
{
    public Guid Id { get; set; }
}