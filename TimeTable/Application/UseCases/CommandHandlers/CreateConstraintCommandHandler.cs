using Application.UseCases.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Common;
using MediatR;

namespace Application.UseCases.CommandHandlers
{
    public class CreateConstraintCommandHandler : IRequestHandler<CreateConstraintCommand, Result<Guid>>
    {
        private readonly IConstraintRepository repository;
        private readonly IMapper mapper;

        public CreateConstraintCommandHandler(IConstraintRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateConstraintCommand request, CancellationToken cancellationToken)
        {
            var constraint = mapper.Map<Constraint>(request);
            var result = await repository.AddAsync(constraint);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }

            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}