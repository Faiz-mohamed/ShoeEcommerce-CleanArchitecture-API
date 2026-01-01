using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Authentication.Commands.ChangePassword;
using ShoeEcommerce.Application.Features.Authentication.Commands.Login;
using ShoeEcommerce.Application.Features.Authentication.Commands.Logout;
using ShoeEcommerce.Application.Features.Authentication.Commands.RefreshToken;
using ShoeEcommerce.Application.Features.Authentication.Commands.Register;
using ShoeEcommerce.Application.Features.Authentication.DTOs;
using System.Security.Claims;

namespace ShoeEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterCommandHandler _registerHandler;
    private readonly LoginCommandHandler _loginHandler;
    private readonly RefreshTokenCommandHandler _refreshTokenHandler;
    private readonly LogoutCommandHandler _logoutHandler;
    private readonly ChangePasswordCommandHandler _changePasswordHandler;

    public AuthController(
        RegisterCommandHandler registerHandler,
        LoginCommandHandler loginHandler,
        RefreshTokenCommandHandler refreshTokenHandler,
        LogoutCommandHandler logoutHandler,
        ChangePasswordCommandHandler changePasswordHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _logoutHandler = logoutHandler;
        _changePasswordHandler = changePasswordHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand
        {
            Email = request.Email,
            Password = request.Password,
            FullName = request.FullName,
            Username = request.Username,
            PhoneNumber = request.PhoneNumber,
            PhoneCountryCode = request.PhoneCountryCode,
            IpAddress = GetIpAddress()
        };

        var result = await _registerHandler.Handle(command);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand
        {
            Identifier = request.Identifier,
            Password = request.Password,
            IpAddress = GetIpAddress()
        };

        var result = await _loginHandler.Handle(command);

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken,
            IpAddress = GetIpAddress()
        };

        var result = await _refreshTokenHandler.Handle(command);

        return Ok(result);
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        if (string.IsNullOrEmpty(request?.RefreshToken))
        {
            return BadRequest(new { message = "Refresh token is required" });
        }

        var command = new LogoutCommand
        {
            RefreshToken = request.RefreshToken,
            IpAddress = GetIpAddress()
        };

        await _logoutHandler.Handle(command);

        return Ok(new { message = "Logged out successfully" });
    }

    private string? GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? null;
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _changePasswordHandler.Handle(request);

        return Ok(result);
    }
}