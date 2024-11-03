using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetConstraintByIdQueryHandler : IRequestHandler<GetConstraintByIdQuery, ConstraintDTO>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public GetConstraintByIdQueryHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<ConstraintDTO> Handle(GetConstraintByIdQuery request, CancellationToken cancellationToken)
        {
            var constraint = await repository.GetByIdAsync(request.Id);
            return mapper.Map<ConstraintDTO>(constraint);
        }
    }
}