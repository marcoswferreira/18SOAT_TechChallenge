using Domain.Interfaces.Repositories;

namespace Application.UseCases.Users;

public class DeleteUserUseCase(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"Usuário com o ID '{id}' não foi encontrado.");
        }

        user.RevokeRefreshToken();
        await _userRepository.DeleteAsync(user, cancellationToken);
    }
}
