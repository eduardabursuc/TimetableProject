using Application.DTOs;
using Application.UseCases.Queries.GroupQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.GroupQueryHandlers
{
    public class GetGroupByIdQueryHandler(IGroupRepository repository, IMapper mapper)
        : IRequestHandler<GetGroupByIdQuery, Result<GroupDto>>
    {
        public async Task<Result<GroupDto>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);
    
            if (!result.IsSuccess) return Result<GroupDto>.Failure(result.ErrorMessage);
    
            var groupDto = mapper.Map<GroupDto>(result.Data);
            return Result<GroupDto>.Success(groupDto);
        }
    }
}

