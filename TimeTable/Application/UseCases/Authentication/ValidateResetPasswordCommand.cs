using Domain.Common;
using MediatR;

namespace Application.UseCases.Authentication;


public class ValidateResetPasswordCommand : IRequest<Result<string>>
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
}