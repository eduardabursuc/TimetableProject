using Application.UseCases.Commands.TimetableCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.TimetableCommandHandlers
{
    public class DeleteTimetableCommandHandler(ITimetableRepository repository, IMapper mapper)
        : IRequestHandler<DeleteTimetableCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteTimetableCommand request, CancellationToken cancellationToken)
        {
            var result = await repository.DeleteAsync(request.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}