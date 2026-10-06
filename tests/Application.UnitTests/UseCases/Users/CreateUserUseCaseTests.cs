using Application.UseCases.Users;
using Application.UseCases.Users.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace Application.UnitTests.UseCases.Users;

public class CreateUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _useCase = new CreateUserUseCase(_userRepository, _passwordHasher);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldCreateUserAndReturnOutput()
    {
        // Arrange
        var input = new CreateUserInput("test@example.com", "Password123!", Roles.Admin);
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(false);
        _passwordHasher.Hash(input.Password).Returns("hashed_password");

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal(Roles.Admin, result.Role);
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u => u.Email == "test@example.com" && u.Role == Roles.Admin), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var input = new CreateUserInput("existing@example.com", "Password123!", Roles.User);
        _userRepository.ExistsByEmailAsync(input.Email, Arg.Any<CancellationToken>()).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidRole_ShouldThrowArgumentException()
    {
        // Arrange
        var input = new CreateUserInput("test@example.com", "Password123!", "InvalidRole");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
