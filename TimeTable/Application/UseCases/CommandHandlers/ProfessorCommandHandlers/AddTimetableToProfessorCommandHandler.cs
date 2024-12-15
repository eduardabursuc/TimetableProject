using Application.UseCases.Commands.ProfessorCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.ProfessorCommandHandlers;

public class AddTimetableToProfessorCommandHandler(IProfessorRepository repository, IMapper mapper)
    : IRequestHandler<AddTimetableToProfessorCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(AddTimetableToProfessorCommand request, CancellationToken cancellationToken)
    {
        var result = await repository.AddTimetableAsync(request.Id, request.TimetableId);
        return result.IsSuccess ? Result<Unit>.Success(result.Data) : Result<Unit>.Failure(result.ErrorMessage);
    }
}