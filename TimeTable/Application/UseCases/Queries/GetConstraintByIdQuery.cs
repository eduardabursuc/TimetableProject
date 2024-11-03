using Application.DTOs;
using MediatR;

namespace Application.UseCases.Queries
{
    public class GetConstraintByIdQuery : IRequest<ConstraintDTO>
    {
        public Guid Id { get; set; }
    }
}