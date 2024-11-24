using Application.DTOs;
using Application.UseCases.Queries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers
{
    public class GetProfessorByIdQueryHandler(IProfessorRepository repository, IMapper mapper)
        : IRequestHandler<GetProfessorByIdQuery, Result<ProfessorDto>>
    {
        public async Task<Result<ProfessorDto>> Handle(GetProfessorByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.GetByIdAsync(request.Id);

            if (!result.IsSuccess) return Result<ProfessorDto>.Failure(result.ErrorMessage);

            var professorDto = mapper.Map<ProfessorDto>(result.Data);
            return Result<ProfessorDto>.Success(professorDto);
        }
    }
}