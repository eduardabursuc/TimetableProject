using Application.UseCases.Commands;
using Domain.Common;
using MediatR;
using System;

namespace Application.UseCases.Commands
{
    public class UpdateProfessorCommand : CreateProfessorCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}