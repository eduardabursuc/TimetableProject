using Application.UseCases.Commands;
using Domain.Common;
using Domain.Entities;
using MediatR;
using System;

namespace Application.UseCases.Commands
{
    public class UpdateConstraintCommand : CreateConstraintCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}
