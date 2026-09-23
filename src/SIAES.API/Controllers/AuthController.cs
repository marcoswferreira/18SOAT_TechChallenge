using Application.UseCases.Auth;
using Application.UseCases.Auth.Dto;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[Route("api/{version}/[controller]")]
public class AuthController(AuthenticateUserUseCase authenticateUserUseCase) : ControllerBase
{
    private readonly AuthenticateUserUseCase _authenticateUserUseCase = authenticateUserUseCase;

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] AuthInput request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticateUserUseCase.ExecuteAsync(request, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new { accessToken = result.AccessToken });
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