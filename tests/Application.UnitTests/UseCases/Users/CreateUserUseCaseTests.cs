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
    public async Task ExecuteAsync_WithValidSingleRole_ShouldCreateUserAndReturnOutput()
    {
        var input = new CreateUserInput("test@example.com", "Password123!", [Roles.Admin]);
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Single(result.Roles);
        Assert.Contains(Roles.Admin, result.Roles);
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Email == "test@example.com" && u.HasRole(Roles.Admin)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleRoles_ShouldCreateUserWithAllRoles()
    {
        var input = new CreateUserInput("test@example.com", "Password123!", [Roles.Admin, Roles.Manager]);
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.Equal(2, result.Roles.Count);
        Assert.Contains(Roles.Admin, result.Roles);
        Assert.Contains(Roles.Manager, result.Roles);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateRoles_ShouldDeduplicateAndCreateUser()
    {
        var input = new CreateUserInput("test@example.com", "Password123!", [Roles.Admin, Roles.Admin]);
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.Single(result.Roles);
        Assert.Contains(Roles.Admin, result.Roles);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyRoles_ShouldDefaultToUserRole()
    {
        var input = new CreateUserInput("test@example.com", "Password123!", []);
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        Assert.Single(result.Roles);
        Assert.Contains(Roles.User, result.Roles);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHashPassword()
    {
        var input = new CreateUserInput("test@example.com", "Password123!", [Roles.User]);
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
        var input = new CreateUserInput("existing@example.com", "Password123!", [Roles.User]);
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidRole_ShouldThrowArgumentException()
    {
        var input = new CreateUserInput("test@example.com", "Password123!", ["InvalidRole"]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithPartiallyInvalidRoles_ShouldThrowArgumentException()
    {
        var input = new CreateUserInput("test@example.com", "Password123!", [Roles.Admin, "Ghost"]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Theory]
    [InlineData("", "Password123!")]
    [InlineData("   ", "Password123!")]
    [InlineData("test@example.com", "")]
    public async Task ExecuteAsync_WithInvalidEmailOrPassword_ShouldThrowArgumentException(string email, string password)
    {
        var input = new CreateUserInput(email, password, [Roles.User]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldNotCallRepository()
    {
        var input = new CreateUserInput("", "Password123!", [Roles.User]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
