using Application.DTOs;
using Application.UseCases.Queries.ConstraintQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.ConstraintQueryHandlers
{
    public class GetConstraintByIdQueryHandler(IConstraintRepository repository, IMapper mapper)
        : IRequestHandler<GetConstraintByIdQuery, Result<ConstraintDto>>
    {
        public async Task<Result<ConstraintDto>> Handle(GetConstraintByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);

            if (!result.IsSuccess) return Result<ConstraintDto>.Failure(result.ErrorMessage);
            
            var constraintDto = mapper.Map<ConstraintDto>(result.Data);
            return Result<ConstraintDto>.Success(constraintDto);
        }
    }
}