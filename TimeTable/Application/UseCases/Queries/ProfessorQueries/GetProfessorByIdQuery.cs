using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.ProfessorQueries
{
    public class GetProfessorByIdQuery : IRequest<Result<ProfessorDto>>
    {
        public Guid Id { get; init; }
    }
}