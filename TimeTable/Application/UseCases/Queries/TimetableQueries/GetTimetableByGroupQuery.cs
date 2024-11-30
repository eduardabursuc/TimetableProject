using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.TimetableQueries
{
    public class GetTimetableByGroupQuery : IRequest<Result<TimetableDto>>
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
    }
}

