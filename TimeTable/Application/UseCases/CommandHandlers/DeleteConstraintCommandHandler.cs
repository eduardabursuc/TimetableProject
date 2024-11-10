using Application.UseCases.Commands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class DeleteConstraintCommandHandler : IRequestHandler<DeleteConstraintCommand, Result<Unit>>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;
        
        public DeleteConstraintCommandHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(DeleteConstraintCommand request, CancellationToken cancellationToken)
        {
            var constraint = mapper.Map<Constraint>(request);
            var result = await repository.DeleteAsync(constraint.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}