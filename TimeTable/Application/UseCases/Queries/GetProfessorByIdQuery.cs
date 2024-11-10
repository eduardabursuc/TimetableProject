using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetProfessorByIdQuery : IRequest<Result<ProfessorDto>>
    {
        public Guid Id { get; set; }
    }
}