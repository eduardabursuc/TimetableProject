using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.ConstraintQueries;

public class GetAllForProfessorQuery : IRequest<Result<List<ConstraintDto>>>
{
    public required string ProfessorEmail { get; init; }
    public required Guid TimetableId { get; init; }
}