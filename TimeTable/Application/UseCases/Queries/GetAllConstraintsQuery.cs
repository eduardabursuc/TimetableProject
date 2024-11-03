using Application.DTOs;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetAllConstraintsQuery : IRequest<List<ConstraintDTO>>
    {
    }
}