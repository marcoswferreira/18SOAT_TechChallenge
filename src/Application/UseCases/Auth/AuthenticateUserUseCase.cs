using Application.UseCases.Auth.Dto;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.UseCases.Auth;

public class AuthenticateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthenticateUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthOutput> ExecuteAsync(AuthInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(input.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(input.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        return await GenerateAndSaveTokensAsync(user, cancellationToken);
    }

    private async Task<AuthOutput> GenerateAndSaveTokensAsync(User user, CancellationToken cancellationToken)
    {
        string accessToken = _tokenService.GenerateAccessToken(user);
        string rawRefreshToken = _tokenService.GenerateRefreshToken();

        string refreshTokenHash = _tokenService.HashToken(rawRefreshToken);
        user.SetRefreshToken(refreshTokenHash, TimeSpan.FromDays(7));

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthOutput(accessToken, rawRefreshToken);
    }
}
