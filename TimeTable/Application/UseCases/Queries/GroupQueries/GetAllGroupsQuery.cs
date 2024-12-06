using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.GroupQueries
{
    public class GetAllGroupsQuery : IRequest<Result<List<GroupDto>>>
    {
    }
}