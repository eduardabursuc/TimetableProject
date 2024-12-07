using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.RoomQueries;

public class GetRoomByIdQuery : IRequest<Result<RoomDto>>
{
    public Guid Id { get; init; }
}