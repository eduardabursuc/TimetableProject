using Application.UseCases.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var userId = await _mediator.Send(command);
        if (!userId.IsSuccess) return Unauthorized(userId.ErrorMessage);
        return Ok(new { UserId = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var token = await _mediator.Send(command);
        if (!token.IsSuccess) return Unauthorized(token.ErrorMessage);
        return Ok(new { Token = token.Data });
    }
}