using Application.UseCases.Commands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;

using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class DeleteProfessorCommandHandler : IRequestHandler<DeleteProfessorCommand, Result<Unit>>
    {
        private readonly IProfessorRepository repository;
        private readonly IMapper mapper;
        
        public DeleteProfessorCommandHandler(IProfessorRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(DeleteProfessorCommand request, CancellationToken cancellationToken)
        {
            var professor = mapper.Map<Professor>(request);
            var result = await repository.DeleteAsync(professor.Id);
            return result.IsSuccess ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(result.ErrorMessage);
        }
    }
}