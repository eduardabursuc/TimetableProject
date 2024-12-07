using Application.Utils;
using Domain.Common;
using Application.DTOs;
using MediatR;

namespace Application.UseCases.Queries.TimetableQueries
{
    public class GetFilteredTimetablesQuery : IRequest<Result<PagedResult<TimetableDto>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string UserEmail { get; set; }
    }
}

