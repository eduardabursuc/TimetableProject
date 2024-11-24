using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.ConstraintQueries
{
    public class GetConstraintByIdQuery : IRequest<Result<ConstraintDto>>
    {
        public Guid Id { get; init; }
    }
}