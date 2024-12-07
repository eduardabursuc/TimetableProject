using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.TimetableQueries
{
    public class GetAllTimetablesQuery : IRequest<Result<List<TimetableDto>>>
    {
        public required string UserEmail { get; init; }
    }
}

