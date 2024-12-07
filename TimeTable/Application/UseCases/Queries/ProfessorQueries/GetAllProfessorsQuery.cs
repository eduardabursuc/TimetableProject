using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.ProfessorQueries
{
    public class GetAllProfessorsQuery : IRequest<Result<List<ProfessorDto>>>
    {
        public required string UserEmail { get; init; }
    }
}