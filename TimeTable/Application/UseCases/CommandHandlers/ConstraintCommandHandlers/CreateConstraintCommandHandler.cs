using Application.UseCases.Commands.ConstraintCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.ConstraintCommandHandlers
{
    public class CreateConstraintCommandHandler(IConstraintRepository repository, IMapper mapper)
        : IRequestHandler<CreateConstraintCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateConstraintCommand request, CancellationToken cancellationToken)
        {
            var constraint = mapper.Map<Constraint>(request);
            var result = await repository.AddAsync(constraint);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}