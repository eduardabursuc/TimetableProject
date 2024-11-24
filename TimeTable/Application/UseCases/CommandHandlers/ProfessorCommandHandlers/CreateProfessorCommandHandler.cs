using Application.UseCases.Commands.ProfessorCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.ProfessorCommandHandlers
{
    public class CreateProfessorCommandHandler(IProfessorRepository repository, IMapper mapper)
        : IRequestHandler<CreateProfessorCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateProfessorCommand request, CancellationToken cancellationToken)
        {
            var professor = mapper.Map<Professor>(request);
            var result = await repository.AddAsync(professor);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}