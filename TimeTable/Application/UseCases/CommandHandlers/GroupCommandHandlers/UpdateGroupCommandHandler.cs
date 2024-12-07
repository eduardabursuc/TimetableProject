using Application.UseCases.Commands.GroupCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.GroupCommandHandlers
{
    public class UpdateGroupCommandHandler(IGroupRepository repository, IMapper mapper)
        : IRequestHandler<UpdateGroupCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = mapper.Map<Group>(request);
            var result = await repository.UpdateAsync(group);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}