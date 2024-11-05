using Application.UseCases.Commands;
using Domain.Common;
using Domain.Entities;
using MediatR;

public class UpdateConstraintCommand : CreateConstraintCommand, IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
}