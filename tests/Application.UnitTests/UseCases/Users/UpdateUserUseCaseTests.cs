using Application.UseCases.Users;
using Application.UseCases.Users.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Users;

public class UpdateUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UpdateUserUseCase _useCase;

    public UpdateUserUseCaseTests()
    {
        _useCase = new UpdateUserUseCase(_userRepository);
    }

    // Helper para criar um usuário com roles carregadas
    private static User CreateUser(string email, params string[] roles) =>
        new(email, "hash", roles);

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldUpdateEmailAndRoles()
    {
        var userId = Guid.NewGuid();
        var existingUser = CreateUser("old@example.com", Roles.User);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);
        _userRepository.ExistsByEmailAsync("new@example.com", Arg.Any<CancellationToken>()).Returns(false);

        var input = new UpdateUserInput("new@example.com", [Roles.Admin]);

        var result = await _useCase.ExecuteAsync(userId, input, CancellationToken.None);

        Assert.Equal("new@example.com", result.Email);
        Assert.Single(result.Roles);
        Assert.Contains(Roles.Admin, result.Roles);
        await _userRepository.Received(1).UpdateAsync(existingUser, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleRoles_ShouldUpdateAllRoles()
    {
        var userId = Guid.NewGuid();
        var existingUser = CreateUser("user@example.com", Roles.User);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);

        var input = new UpdateUserInput("user@example.com", [Roles.Admin, Roles.Manager]);

        var result = await _useCase.ExecuteAsync(userId, input, CancellationToken.None);

        Assert.Equal(2, result.Roles.Count);
        Assert.Contains(Roles.Admin, result.Roles);
        Assert.Contains(Roles.Manager, result.Roles);
    }

    [Fact]
    public async Task ExecuteAsync_WithSameEmail_ShouldNotCheckEmailUniqueness()
    {
        var userId = Guid.NewGuid();
        var existingUser = CreateUser("same@example.com", Roles.User);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);

        var input = new UpdateUserInput("same@example.com", [Roles.Admin]);

        await _useCase.ExecuteAsync(userId, input, CancellationToken.None);

        // Email é igual ao atual — não deve verificar unicidade
        await _userRepository.DidNotReceive().ExistsByEmailAsync(
            Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateRoles_ShouldDeduplicateRoles()
    {
        var userId = Guid.NewGuid();
        var existingUser = CreateUser("user@example.com", Roles.User);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);

        var input = new UpdateUserInput("user@example.com", [Roles.Admin, Roles.Admin]);

        var result = await _useCase.ExecuteAsync(userId, input, CancellationToken.None);

        Assert.Single(result.Roles);
    }

    // -------------------------------------------------------------------------
    // Error cases
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ShouldThrowKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var input = new UpdateUserInput("new@example.com", [Roles.Admin]);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _useCase.ExecuteAsync(userId, input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidRole_ShouldThrowArgumentException()
    {
        var userId = Guid.NewGuid();
        var existingUser = CreateUser("user@example.com", Roles.User);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);

        var input = new UpdateUserInput("user@example.com", ["InvalidRole"]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(userId, input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var existingUser = CreateUser("old@example.com", Roles.User);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);
        _userRepository.ExistsByEmailAsync("taken@example.com", Arg.Any<CancellationToken>()).Returns(true);

        var input = new UpdateUserInput("taken@example.com", [Roles.Admin]);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.ExecuteAsync(userId, input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ShouldNotCallUpdate()
    {
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var input = new UpdateUserInput("new@example.com", [Roles.Admin]);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _useCase.ExecuteAsync(userId, input, CancellationToken.None));

        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
