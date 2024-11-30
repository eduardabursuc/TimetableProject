using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Queries.TimetableQueries
{
    public class GetTimetableByRoomQuery : IRequest<Result<TimetableDto>>
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; }
    }
}


