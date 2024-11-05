using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetConstraintByIdQueryHandler : IRequestHandler<GetConstraintByIdQuery, Result<ConstraintDTO>>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public GetConstraintByIdQueryHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<ConstraintDTO>> Handle(GetConstraintByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);

            if (!result.IsSuccess) return Result<ConstraintDTO>.Failure(result.ErrorMessage);
            
            var constraintDTO = mapper.Map<ConstraintDTO>(result.Data);
            return Result<ConstraintDTO>.Success(constraintDTO);
        }
    }
}