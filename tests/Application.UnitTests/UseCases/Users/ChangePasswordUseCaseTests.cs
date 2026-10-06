using Application.UseCases.Users;
using Application.UseCases.Users.Dto;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Users;

public class ChangePasswordUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ChangePasswordUseCase _useCase;

    public ChangePasswordUseCaseTests()
    {
        _useCase = new ChangePasswordUseCase(_userRepository, _passwordHasher);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldChangePassword()
    {
        var userId = Guid.NewGuid();
        var input = new ChangePasswordInput("OldPassword123!", "NewPassword123!");
        var user = new User("test@example.com", "old_hash", []);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(input.CurrentPassword, "old_hash").Returns(true);
        _passwordHasher.Hash(input.NewPassword).Returns("new_hash");

        await _useCase.ExecuteAsync(userId, input, CancellationToken.None);

        Assert.Equal("new_hash", user.PasswordHash);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyNewPassword_ShouldThrowArgumentException()
    {
        var input = new ChangePasswordInput("OldPassword123!", "");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid(), input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithUserNotFound_ShouldThrowKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        var input = new ChangePasswordInput("OldPassword123!", "NewPassword123!");

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _useCase.ExecuteAsync(userId, input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidCurrentPassword_ShouldThrowUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();
        var input = new ChangePasswordInput("WrongPassword!", "NewPassword123!");
        var user = new User("test@example.com", "old_hash", []);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(input.CurrentPassword, "old_hash").Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _useCase.ExecuteAsync(userId, input, CancellationToken.None));
    }
}
