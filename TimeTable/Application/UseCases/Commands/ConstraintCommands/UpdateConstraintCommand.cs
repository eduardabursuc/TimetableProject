﻿using Domain.Common;
using MediatR;

namespace Application.UseCases.Commands.ConstraintCommands
{
    public class UpdateConstraintCommand : CreateConstraintCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}