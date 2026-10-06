using Application.UseCases.Auth;
using Application.UseCases.Auth.Dto;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(
    AuthenticateUserUseCase authenticateUserUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    RevokeTokenUseCase revokeTokenUseCase) : ControllerBase
{
    private readonly AuthenticateUserUseCase _authenticateUserUseCase = authenticateUserUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase = refreshTokenUseCase;
    private readonly RevokeTokenUseCase _revokeTokenUseCase = revokeTokenUseCase;

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] AuthInput request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticateUserUseCase.ExecuteAsync(request, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest? request,
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"] ?? request?.RefreshToken;

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { error = "Refresh Token não fornecido." });
        }

        var result = await _refreshTokenUseCase.ExecuteAsync(refreshToken, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest? request,
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"] ?? request?.RefreshToken;

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _revokeTokenUseCase.ExecuteAsync(refreshToken, cancellationToken);
        }

        Response.Cookies.Delete("refreshToken");

        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}

public record RefreshTokenRequest(string? RefreshToken);