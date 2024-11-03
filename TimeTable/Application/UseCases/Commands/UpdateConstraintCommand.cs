using Application.DTOs;
using Application.UseCases.Commands;
using MediatR;

public class UpdateConstraintCommand : CreateConstraintCommand, IRequest<Guid>
{
    public Guid Id { get; set; }
    public UpdateConstraintCommand(ConstraintDTO constraint) : base(constraint)
    {

    }
}