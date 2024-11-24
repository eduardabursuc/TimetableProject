using Application.UseCases.Commands.ConstraintCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.ConstraintCommandHandlers
{
    public class DeleteConstraintCommandHandler(IConstraintRepository repository, IMapper mapper)
        : IRequestHandler<DeleteConstraintCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteConstraintCommand request, CancellationToken cancellationToken)
        {
            var constraint = mapper.Map<Constraint>(request);
            var result = await repository.DeleteAsync(constraint.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}