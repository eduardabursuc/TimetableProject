using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Common;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class UpdateProfessorCommandHandler : IRequestHandler<UpdateProfessorCommand, Result<Guid>>
    {
        private readonly IProfessorRepository repository;
        private readonly IMapper mapper;

        public UpdateProfessorCommandHandler(IProfessorRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(UpdateProfessorCommand request, CancellationToken cancellationToken)
        {
            var professor = mapper.Map<Professor>(request);
            var result = await repository.UpdateAsync(professor);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }

            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}