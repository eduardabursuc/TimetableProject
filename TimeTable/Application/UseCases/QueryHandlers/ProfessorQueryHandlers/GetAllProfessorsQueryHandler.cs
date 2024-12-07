using Application.DTOs;
using Application.UseCases.Queries.ProfessorQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.ProfessorQueryHandlers
{
    public class GetAllProfessorsQueryHandler(IProfessorRepository repository, IMapper mapper)
        : IRequestHandler<GetAllProfessorsQuery, Result<List<ProfessorDto>>>
    {
        public async Task<Result<List<ProfessorDto>>> Handle(GetAllProfessorsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetAllAsync(request.UserEmail);

            if (!result.IsSuccess) return Result<List<ProfessorDto>>.Failure(result.ErrorMessage);
            
            var professorDtOs = mapper.Map<List<ProfessorDto>>(result.Data) ?? new List<ProfessorDto>();
            return Result<List<ProfessorDto>>.Success(professorDtOs);
        }
    }
}