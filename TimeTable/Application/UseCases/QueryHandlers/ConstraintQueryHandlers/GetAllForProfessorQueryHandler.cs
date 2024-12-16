using Application.DTOs;
using Application.UseCases.Queries.ConstraintQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.QueryHandlers.ConstraintQueryHandlers;

public class GetAllForProfessorQueryHandler(IConstraintRepository repository, IMapper mapper)
    : IRequestHandler<GetAllForProfessorQuery, Result<List<ConstraintDto>>>
{
    public async Task<Result<List<ConstraintDto>>> Handle(GetAllForProfessorQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllForProfessorAsync(request.ProfessorEmail, request.TimetableId);

        if (!result.IsSuccess) return Result<List<ConstraintDto>>.Failure(result.ErrorMessage);
            
        var constraintDtOs = mapper.Map<List<ConstraintDto>>(result.Data) ?? [];
        return Result<List<ConstraintDto>>.Success(constraintDtOs);
    }
}