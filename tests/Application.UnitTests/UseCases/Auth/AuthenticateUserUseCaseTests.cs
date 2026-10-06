using Application.UseCases.Auth;
using Application.UseCases.Auth.Dto;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using NSubstitute;

namespace Application.UnitTests.UseCases.Auth;

public class AuthenticateUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly AuthenticateUserUseCase _useCase;

    public AuthenticateUserUseCaseTests()
    {
        _useCase = new AuthenticateUserUseCase(_userRepository, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ShouldReturnAuthOutput()
    {
        var input = new AuthInput("test@example.com", "Password123!");
        var user = new User("test@example.com", "hashed_password", []);
        
        _userRepository.GetByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(input.Password, user.PasswordHash).Returns(true);
        _tokenService.GenerateAccessToken(user).Returns("access_token");
        _tokenService.GenerateRefreshToken().Returns("refresh_token");
        _tokenService.HashToken("refresh_token").Returns("hashed_refresh");

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("access_token", result.AccessToken);
        Assert.Equal("refresh_token", result.RefreshToken);

        await _userRepository.Received(1).UpdateAsync(
            Arg.Is<User>(u => u.RefreshTokenHash == "hashed_refresh"),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldThrowUnauthorizedAccessException()
    {
        var input = new AuthInput("invalid@example.com", "Password123!");
        _userRepository.GetByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        var input = new AuthInput("test@example.com", "WrongPassword!");
        var user = new User("test@example.com", "hashed_password", []);
        
        _userRepository.GetByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(input.Password, user.PasswordHash).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
