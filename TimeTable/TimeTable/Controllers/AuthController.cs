using Application.UseCases.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var userId = await mediator.Send(command);
        if (!userId.IsSuccess) return Unauthorized(userId.ErrorMessage);
        return Ok(new { UserId = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var token = await mediator.Send(command);
        if (!token.IsSuccess) return Unauthorized(token.ErrorMessage);
        return Ok(new { Token = token.Data });
    }
    
    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
    {
        var token = await mediator.Send(command);
        if (!token.IsSuccess) return Unauthorized(token.ErrorMessage);
        return Ok(new { Token = token.Data });
    }

    [HttpPost("resetPassword")]
    public async Task<ActionResult<string>> ResetPassword(ResetPasswordCommand command)
    {
        var token = await mediator.Send(command);
        if (!token.IsSuccess) return Unauthorized(token.ErrorMessage);
        return Ok(new { Token = token.Data });
    }
    
    [Authorize]
    [HttpPost("validateResetPassword")]
    public async Task<ActionResult<string>> ValidateResetPassword(ValidateResetPasswordCommand command)
    {
        var token = await mediator.Send(command);
        if (!token.IsSuccess) return Unauthorized(token.ErrorMessage);
        return Ok(new { Token = token.Data });
    }
    
}