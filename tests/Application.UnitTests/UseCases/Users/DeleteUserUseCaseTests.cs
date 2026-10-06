using Application.UseCases.Users;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Users;

public class DeleteUserUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly DeleteUserUseCase _useCase;

    public DeleteUserUseCaseTests()
    {
        _useCase = new DeleteUserUseCase(_userRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldDeleteUser()
    {
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", []);
        user.SetRefreshToken("refresh_hash", TimeSpan.FromDays(7));

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        await _useCase.ExecuteAsync(userId, CancellationToken.None);

        Assert.Null(user.RefreshTokenHash); // Ensure token is revoked
        await _userRepository.Received(1).DeleteAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithUserNotFound_ShouldThrowKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _useCase.ExecuteAsync(userId, CancellationToken.None));

        await _userRepository.DidNotReceive().DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
