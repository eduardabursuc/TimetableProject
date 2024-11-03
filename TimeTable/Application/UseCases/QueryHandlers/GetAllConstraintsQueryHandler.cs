using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetAllConstraintsQueryHandler : IRequestHandler<GetAllConstraintsQuery, List<ConstraintDTO>>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public GetAllConstraintsQueryHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<ConstraintDTO>> Handle(GetAllConstraintsQuery request, CancellationToken cancellationToken)
        {
            var constraints = await repository.GetAllAsync();
            return mapper.Map<List<ConstraintDTO>>(constraints) ?? new List<ConstraintDTO>();
        }
    }
}