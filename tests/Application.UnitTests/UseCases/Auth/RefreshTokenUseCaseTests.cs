using Application.UseCases.Auth;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using NSubstitute;

namespace Application.UnitTests.UseCases.Auth;

public class RefreshTokenUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly RefreshTokenUseCase _useCase;

    public RefreshTokenUseCaseTests()
    {
        _useCase = new RefreshTokenUseCase(_userRepository, _tokenService);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        var rawRefreshToken = "old_raw_refresh";
        var user = new User("test@example.com", "hash", []);
        user.SetRefreshToken("old_hashed_refresh", TimeSpan.FromDays(7));

        _tokenService.HashToken(rawRefreshToken).Returns("old_hashed_refresh");
        _userRepository.GetByRefreshTokenAsync("old_hashed_refresh", Arg.Any<CancellationToken>()).Returns(user);
        
        _tokenService.GenerateAccessToken(user).Returns("new_access_token");
        _tokenService.GenerateRefreshToken().Returns("new_raw_refresh");
        _tokenService.HashToken("new_raw_refresh").Returns("new_hashed_refresh");

        var result = await _useCase.ExecuteAsync(rawRefreshToken, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("new_access_token", result.AccessToken);
        Assert.Equal("new_raw_refresh", result.RefreshToken);

        await _userRepository.Received(1).UpdateAsync(
            Arg.Is<User>(u => u.RefreshTokenHash == "new_hashed_refresh"),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidTokenHash_ShouldThrowUnauthorizedAccessException()
    {
        var rawRefreshToken = "invalid_refresh";
        _tokenService.HashToken(rawRefreshToken).Returns("invalid_hashed");
        _userRepository.GetByRefreshTokenAsync("invalid_hashed", Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _useCase.ExecuteAsync(rawRefreshToken, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithExpiredToken_ShouldRevokeAndThrowUnauthorizedAccessException()
    {
        var rawRefreshToken = "expired_raw_refresh";
        var user = new User("test@example.com", "hash", []);
        user.SetRefreshToken("expired_hashed_refresh", TimeSpan.FromDays(-1)); // expired

        _tokenService.HashToken(rawRefreshToken).Returns("expired_hashed_refresh");
        _userRepository.GetByRefreshTokenAsync("expired_hashed_refresh", Arg.Any<CancellationToken>()).Returns(user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _useCase.ExecuteAsync(rawRefreshToken, CancellationToken.None));

        Assert.Null(user.RefreshTokenHash);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }
}
