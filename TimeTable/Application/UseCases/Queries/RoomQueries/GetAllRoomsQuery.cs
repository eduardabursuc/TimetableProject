using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.RoomQueries
{
    public class GetAllRoomsQuery : IRequest<Result<List<RoomDto>>>
    {
        public required string UserEmail { get; init; }
    }
}