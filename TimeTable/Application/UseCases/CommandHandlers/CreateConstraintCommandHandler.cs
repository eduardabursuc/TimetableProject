using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class CreateConstraintCommandHandler : IRequestHandler<CreateConstraintCommand, Guid>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public CreateConstraintCommandHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(CreateConstraintCommand request, CancellationToken cancellationToken)
        {
            var constraint = mapper.Map<Constraint>(request);
            return await repository.AddAsync(constraint);
        }
    }
}