using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.UseCases.Auth;

public class RevokeTokenUseCase(IUserRepository userRepository, ITokenService tokenService)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenService _tokenService = tokenService;

    public async Task ExecuteAsync(string rawRefreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rawRefreshToken)) return;

        string refreshTokenHash = _tokenService.HashToken(rawRefreshToken);
        var user = await _userRepository.GetByRefreshTokenAsync(refreshTokenHash, cancellationToken);

        if (user is not null)
        {
            user.RevokeRefreshToken();
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
