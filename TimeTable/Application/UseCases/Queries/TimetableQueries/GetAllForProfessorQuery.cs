using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.TimetableQueries;

public class GetAllForProfessorQuery: IRequest<Result<List<TimetableDto>>>
{
    public required string ProfessorEmail { get; init; }
}