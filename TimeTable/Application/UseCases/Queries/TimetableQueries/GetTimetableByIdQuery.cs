using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.TimetableQueries
{
    public class GetTimetableByIdQuery : IRequest<Result<TimetableDto>>
    {
        public Guid Id { get; init; }
    }
}

