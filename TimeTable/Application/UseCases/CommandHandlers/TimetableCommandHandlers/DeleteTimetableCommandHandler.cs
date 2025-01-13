using Application.UseCases.Commands.TimetableCommands;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.TimetableCommandHandlers
{
    public class DeleteTimetableCommandHandler(ITimetableRepository repository)
        : IRequestHandler<DeleteTimetableCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteTimetableCommand request, CancellationToken cancellationToken)
        {
            var result = await repository.DeleteAsync(request.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}