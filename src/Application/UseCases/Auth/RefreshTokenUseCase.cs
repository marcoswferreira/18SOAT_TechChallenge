using Application.UseCases.Auth.Dto;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.UseCases.Auth;

public class RefreshTokenUseCase(IUserRepository userRepository, ITokenService tokenService)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<AuthOutput> ExecuteAsync(string rawRefreshToken, CancellationToken cancellationToken)
    {
        string refreshTokenHash = _tokenService.HashToken(rawRefreshToken);
        var user = await _userRepository.GetByRefreshTokenAsync(refreshTokenHash, cancellationToken);

        if (user is null || !user.IsRefreshTokenValid(refreshTokenHash))
        {
            // Segurança: Se o token for inválido, garante revogação para mitigar reuso por atacantes
            user?.RevokeRefreshToken();
            if (user is not null)
                await _userRepository.UpdateAsync(user, cancellationToken);

            throw new UnauthorizedAccessException("Refresh Token inválido ou expirado.");
        }

        // Rotação: Gera um novo par e substitui o anterior no banco
        string newAccessToken = _tokenService.GenerateAccessToken(user);
        string newRawRefreshToken = _tokenService.GenerateRefreshToken();

        string newRefreshTokenHash = _tokenService.HashToken(newRawRefreshToken);
        user.SetRefreshToken(newRefreshTokenHash, TimeSpan.FromDays(7));

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthOutput(newAccessToken, newRawRefreshToken);
    }
}