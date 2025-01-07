using Domain.Common;
using MediatR;

namespace Application.UseCases.Authentication;

public class ResetPasswordCommand : IRequest<Result<string>>
{
    public required string Email { get; set; }
}