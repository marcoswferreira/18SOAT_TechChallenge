using Application.UseCases.Auth;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using NSubstitute;

namespace Application.UnitTests.UseCases.Auth;

public class RevokeTokenUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly RevokeTokenUseCase _useCase;

    public RevokeTokenUseCaseTests()
    {
        _useCase = new RevokeTokenUseCase(_userRepository, _tokenService);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidToken_ShouldRevokeToken()
    {
        var rawRefreshToken = "valid_raw_refresh";
        var user = new User("test@example.com", "hash", []);
        user.SetRefreshToken("valid_hashed_refresh", TimeSpan.FromDays(7));

        _tokenService.HashToken(rawRefreshToken).Returns("valid_hashed_refresh");
        _userRepository.GetByRefreshTokenAsync("valid_hashed_refresh", Arg.Any<CancellationToken>()).Returns(user);

        await _useCase.ExecuteAsync(rawRefreshToken, CancellationToken.None);

        Assert.Null(user.RefreshTokenHash);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidToken_ShouldNotThrowOrUpdate()
    {
        var rawRefreshToken = "invalid_raw_refresh";

        _tokenService.HashToken(rawRefreshToken).Returns("invalid_hashed_refresh");
        _userRepository.GetByRefreshTokenAsync("invalid_hashed_refresh", Arg.Any<CancellationToken>()).Returns((User?)null);

        await _useCase.ExecuteAsync(rawRefreshToken, CancellationToken.None);

        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithEmptyToken_ShouldReturnImmediately(string rawRefreshToken)
    {
        await _useCase.ExecuteAsync(rawRefreshToken, CancellationToken.None);

        _tokenService.DidNotReceive().HashToken(Arg.Any<string>());
        await _userRepository.DidNotReceive().GetByRefreshTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
