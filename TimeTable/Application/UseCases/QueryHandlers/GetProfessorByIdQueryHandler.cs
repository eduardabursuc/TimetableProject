using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetProfessorByIdQueryHandler : IRequestHandler<GetProfessorByIdQuery, Result<ProfessorDTO>>
    {
        private readonly IProfessorRepository repository;
        private readonly IMapper mapper;

        public GetProfessorByIdQueryHandler(IProfessorRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<ProfessorDTO>> Handle(GetProfessorByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);

            if (!result.IsSuccess) return Result<ProfessorDTO>.Failure(result.ErrorMessage);

            var professorDTO = mapper.Map<ProfessorDTO>(result.Data);
            return Result<ProfessorDTO>.Success(professorDTO);
        }
    }
}