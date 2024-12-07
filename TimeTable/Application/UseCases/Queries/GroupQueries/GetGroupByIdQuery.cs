using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.GroupQueries
{
    public class GetGroupByIdQuery : IRequest<Result<GroupDto>>
    {
        public Guid Id { get; init; }
    }
}

