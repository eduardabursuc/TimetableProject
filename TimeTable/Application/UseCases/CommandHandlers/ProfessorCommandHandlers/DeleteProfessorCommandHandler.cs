using Application.UseCases.Commands.ProfessorCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.ProfessorCommandHandlers
{
    public class DeleteProfessorCommandHandler(IProfessorRepository repository, IMapper mapper)
        : IRequestHandler<DeleteProfessorCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteProfessorCommand request, CancellationToken cancellationToken)
        {
            var professor = mapper.Map<Professor>(request);
            var result = await repository.DeleteAsync(professor.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}