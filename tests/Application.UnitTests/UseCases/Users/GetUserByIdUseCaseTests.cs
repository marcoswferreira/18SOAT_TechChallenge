using Application.UseCases.Users;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Users;

public class GetUserByIdUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetUserByIdUseCase _useCase;

    public GetUserByIdUseCaseTests()
    {
        _useCase = new GetUserByIdUseCase(_userRepository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldReturnUserOutput()
    {
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hash", []);
        
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _useCase.ExecuteAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WithUserNotFound_ShouldThrowKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _useCase.ExecuteAsync(userId, CancellationToken.None));
    }
}
