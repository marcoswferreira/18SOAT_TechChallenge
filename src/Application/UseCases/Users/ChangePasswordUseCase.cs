using Application.UseCases.Users.Dto;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Users;

public class ChangePasswordUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task ExecuteAsync(Guid id, ChangePasswordInput input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.NewPassword))
        {
            throw new ArgumentException("A nova senha não pode ser vazia.");
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"Usuário com o ID '{id}' não foi encontrado.");
        }

        if (!_passwordHasher.Verify(input.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("A senha atual informada é inválida.");
        }

        var newPasswordHash = _passwordHasher.Hash(input.NewPassword);
        user.UpdatePassword(newPasswordHash);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
