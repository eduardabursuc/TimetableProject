using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.TimetableQueries
{
    public class GetTimetableByProfessorQuery : IRequest<Result<TimetableDto>>
    {
        public Guid Id { get; set; }
        public Guid ProfessorId { get; set; }
    }
}
