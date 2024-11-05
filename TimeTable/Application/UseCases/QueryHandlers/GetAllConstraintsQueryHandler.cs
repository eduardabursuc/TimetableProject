using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetAllConstraintsQueryHandler : IRequestHandler<GetAllConstraintsQuery, Result<List<ConstraintDTO>>>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public GetAllConstraintsQueryHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<List<ConstraintDTO>>> Handle(GetAllConstraintsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync();

            if (!result.IsSuccess) return Result<List<ConstraintDTO>>.Failure(result.ErrorMessage);
            
            var constraintDTOs = mapper.Map<List<ConstraintDTO>>(result.Data) ?? new List<ConstraintDTO>();
            return Result<List<ConstraintDTO>>.Success(constraintDTOs);
        }
    }
}