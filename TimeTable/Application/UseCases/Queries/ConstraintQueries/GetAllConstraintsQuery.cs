using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.ConstraintQueries
{
    public class GetAllConstraintsQuery : IRequest<Result<List<ConstraintDto>>>
    {
    }
}