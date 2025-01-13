using Domain.Common;
using MediatR;

namespace Application.UseCases.Authentication;


public class ValidateResetPasswordCommand : IRequest<Result<string>>
{
    public required string Email { get; set; }
    public required string NewPassword { get; set; }
}