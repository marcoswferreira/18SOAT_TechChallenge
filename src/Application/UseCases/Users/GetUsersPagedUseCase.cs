using Application.UseCases.Users.Dto;
using Domain.Common.Paginate;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Users;

public class GetUsersPagedUseCase(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IPaginate<UserOutput>> ExecuteAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var pagedUsers = await _userRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        
        return new Paginate<Domain.Entities.User, UserOutput>(
            pagedUsers,
            users => users.Select(UserOutput.FromEntity));
    }
}
