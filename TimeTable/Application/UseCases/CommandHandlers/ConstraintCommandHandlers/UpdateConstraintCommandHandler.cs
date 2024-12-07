using Application.UseCases.Commands.ConstraintCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.ConstraintCommandHandlers
{
    public class UpdateConstraintCommandHandler(IConstraintRepository repository, IMapper mapper)
        : IRequestHandler<UpdateConstraintCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(UpdateConstraintCommand request, CancellationToken cancellationToken)
        {
            var constraint = mapper.Map<Constraint>(request);

            var result = await repository.UpdateAsync(constraint);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}