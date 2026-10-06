using Application.UseCases.Users;
using Application.UseCases.Users.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace Application.UnitTests.UseCases.Users;

public class UpdateUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UpdateUserUseCase _useCase;

    public UpdateUserUseCaseTests()
    {
        _useCase = new UpdateUserUseCase(_userRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldUpdateUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User("old@example.com", "hash", Roles.User);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);
        _userRepository.ExistsByEmailAsync("new@example.com", Arg.Any<CancellationToken>()).Returns(false);

        var input = new UpdateUserInput("new@example.com", Roles.Admin);

        // Act
        var result = await _useCase.ExecuteAsync(userId, input, CancellationToken.None);

        // Assert
        Assert.Equal("new@example.com", result.Email);
        Assert.Equal(Roles.Admin, result.Role);
        await _userRepository.Received(1).UpdateAsync(existingUser, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var input = new UpdateUserInput("new@example.com", Roles.Admin);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(userId, input, CancellationToken.None));
    }
}
