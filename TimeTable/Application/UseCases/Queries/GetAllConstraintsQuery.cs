using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetAllConstraintsQuery : IRequest<Result<List<ConstraintDTO>>>
    {
    }
}