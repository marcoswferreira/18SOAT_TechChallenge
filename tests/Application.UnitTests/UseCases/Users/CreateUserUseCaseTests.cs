using Application.UseCases.Users;
using Application.UseCases.Users.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Users;

public class CreateUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _useCase = new CreateUserUseCase(_userRepository, _passwordHasher);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");
    }

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldCreateUserWithUserRoleAndReturnOutput()
    {
        var input = new CreateUserInput("test@example.com", "Password123!");
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Single(result.Roles);
        Assert.Contains(Roles.User, result.Roles);
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Email == "test@example.com" && u.HasRole(Roles.User)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNormalizeEmail()
    {
        var input = new CreateUserInput("  Test.User@Example.COM  ", "Password123!");
        _userRepository.ExistsByEmailAsync("test.user@example.com", Arg.Any<CancellationToken>()).Returns(false);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.Equal("test.user@example.com", result.Email);
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Email == "test.user@example.com"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHashPassword()
    {
        var input = new CreateUserInput("test@example.com", "Password123!");
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(false);

        await _useCase.ExecuteAsync(input, CancellationToken.None);

        _passwordHasher.Received(1).Hash("Password123!");
    }

    // -------------------------------------------------------------------------
    // Validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        var input = new CreateUserInput("existing@example.com", "Password123!");
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Theory]
    [InlineData("", "Password123!")]
    [InlineData("   ", "Password123!")]
    [InlineData("test@example.com", "")]
    [InlineData("test@example.com", "   ")]
    public async Task ExecuteAsync_WithInvalidEmailOrPassword_ShouldThrowArgumentException(string email, string password)
    {
        var input = new CreateUserInput(email, password);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldNotCallRepository()
    {
        var input = new CreateUserInput("", "Password123!");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
