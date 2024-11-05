using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetConstraintByIdQuery : IRequest<Result<ConstraintDTO>>
    {
        public Guid Id { get; set; }
    }
}