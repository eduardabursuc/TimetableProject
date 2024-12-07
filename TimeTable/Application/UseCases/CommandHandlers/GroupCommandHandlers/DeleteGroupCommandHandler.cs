using Application.UseCases.Commands.GroupCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.GroupCommandHandlers
{
    public class DeleteGroupCommandHandler(IGroupRepository repository, IMapper mapper)
        : IRequestHandler<DeleteGroupCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var group = mapper.Map<Group>(request);
            var result = await repository.DeleteAsync(group.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}