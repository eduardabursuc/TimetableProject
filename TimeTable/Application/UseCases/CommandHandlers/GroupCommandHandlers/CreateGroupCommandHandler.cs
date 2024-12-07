using Application.UseCases.Commands.GroupCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.GroupCommandHandlers
{
    public class CreateGroupCommandHandler(IGroupRepository repository, IMapper mapper)
        : IRequestHandler<CreateGroupCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = mapper.Map<Group>(request);
            var result = await repository.AddAsync(group);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}