using Application.UseCases.Users;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Users;

public class GetUsersPagedUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetUsersPagedUseCase _useCase;

    public GetUsersPagedUseCaseTests()
    {
        _useCase = new GetUsersPagedUseCase(_userRepository);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnPagedUserOutput()
    {
        var users = new List<User>
        {
            new User("user1@example.com", "hash1", []),
            new User("user2@example.com", "hash2", [])
        };

        var pagedList = Substitute.For<IPaginate<User>>();
        pagedList.Items.Returns(users);
        pagedList.Count.Returns(2);

        _userRepository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>()).Returns(pagedList);

        var result = await _useCase.ExecuteAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("user1@example.com", result.Items[0].Email);
        Assert.Equal("user2@example.com", result.Items[1].Email);
    }
}
