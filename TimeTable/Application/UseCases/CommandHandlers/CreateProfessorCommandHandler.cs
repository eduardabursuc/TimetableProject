using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Common;
using MediatR;
using Domain.Repositories;


namespace Application.UseCases.CommandHandlers
{
    public class CreateProfessorCommandHandler : IRequestHandler<CreateProfessorCommand, Result<Guid>>
    {
        private readonly IProfessorRepository repository;
        private readonly IMapper mapper;

        public CreateProfessorCommandHandler(IProfessorRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateProfessorCommand request, CancellationToken cancellationToken)
        {
            var professor = mapper.Map<Professor>(request);
            var result = await repository.AddAsync(professor);
            
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }

            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}