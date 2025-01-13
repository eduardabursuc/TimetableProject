using Domain.Common;
using MediatR;

namespace Application.UseCases.Authentication;

public class RefreshTokenCommand : IRequest<Result<string>>
{
    public required string Email { get; set; }
}