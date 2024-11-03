using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class UpdateConstraintCommandHandler : IRequestHandler<UpdateConstraintCommand, Guid>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public UpdateConstraintCommandHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(UpdateConstraintCommand request, CancellationToken cancellationToken)
        {
            var constraint = mapper.Map<Constraint>(request);
            await repository.UpdateAsync(constraint);
            return constraint.Id;
        }
    }
}