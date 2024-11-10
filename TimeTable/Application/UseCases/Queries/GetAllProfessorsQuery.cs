using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetAllProfessorsQuery : IRequest<Result<List<ProfessorDTO>>>
    {
    }
}