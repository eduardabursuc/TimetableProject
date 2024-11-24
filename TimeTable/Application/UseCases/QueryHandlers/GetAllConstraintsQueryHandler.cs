using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetAllConstraintsQueryHandler(IConstraintRepository repository, IMapper mapper)
        : IRequestHandler<GetAllConstraintsQuery, Result<List<ConstraintDto>>>
    {
        public async Task<Result<List<ConstraintDto>>> Handle(GetAllConstraintsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync();

            if (!result.IsSuccess) return Result<List<ConstraintDto>>.Failure(result.ErrorMessage);
            
            var constraintDtOs = mapper.Map<List<ConstraintDto>>(result.Data) ?? [];
            return Result<List<ConstraintDto>>.Success(constraintDtOs);
        }
    }
}