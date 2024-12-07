using Application.DTOs;
using Application.UseCases.Queries.GroupQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.GroupQueryHandlers
{
    public class GetAllGroupsQueryHandler(IGroupRepository repository, IMapper mapper)
        : IRequestHandler<GetAllGroupsQuery, Result<List<GroupDto>>>
    {
        public async Task<Result<List<GroupDto>>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync(request.UserEmail);

            if (!result.IsSuccess) return Result<List<GroupDto>>.Failure(result.ErrorMessage);

            var groupDtOs = mapper.Map<List<GroupDto>>(result.Data) ?? [];
            return Result<List<GroupDto>>.Success(groupDtOs);
        }
    }
}