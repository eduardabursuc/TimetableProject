using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetConstraintByIdQueryHandler : IRequestHandler<GetConstraintByIdQuery, Result<ConstraintDto>>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public GetConstraintByIdQueryHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<ConstraintDto>> Handle(GetConstraintByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);

            if (!result.IsSuccess) return Result<ConstraintDto>.Failure(result.ErrorMessage);
            
            var constraintDTO = mapper.Map<ConstraintDto>(result.Data);
            return Result<ConstraintDto>.Success(constraintDTO);
        }
    }
}