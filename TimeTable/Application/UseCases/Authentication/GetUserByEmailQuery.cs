using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Authentication
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public string Email { get; init; }
    }
}