using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetAllProfessorsQueryHandler : IRequestHandler<GetAllProfessorsQuery, Result<List<ProfessorDto>>>
    {
        private readonly IProfessorRepository repository;
        private readonly IMapper mapper;

        public GetAllProfessorsQueryHandler(IProfessorRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<List<ProfessorDto>>> Handle(GetAllProfessorsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync();

            if (!result.IsSuccess) return Result<List<ProfessorDto>>.Failure(result.ErrorMessage);
            
            var professorDTOs = mapper.Map<List<ProfessorDto>>(result.Data) ?? new List<ProfessorDto>();
            return Result<List<ProfessorDto>>.Success(professorDTOs);
        }
    }
}