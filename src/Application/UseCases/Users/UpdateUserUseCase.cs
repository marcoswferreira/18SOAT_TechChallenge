using Application.UseCases.Users.Dto;
using Domain.Constants;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Users;

public class UpdateUserUseCase(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserOutput> ExecuteAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"Usuário com o ID '{id}' não foi encontrado.");
        }

        if (!Roles.IsValid(input.Role))
        {
            throw new ArgumentException($"Role inválida. As roles permitidas são: {string.Join(", ", Roles.All)}.");
        }

        var normalizedNewEmail = input.Email.Trim().ToLowerInvariant();
        if (!user.Email.Equals(normalizedNewEmail, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _userRepository.ExistsByEmailAsync(normalizedNewEmail, cancellationToken);
            if (emailExists)
            {
                throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail '{input.Email}'.");
            }
        }

        user.Update(input.Email, input.Role);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return UserOutput.FromEntity(user);
    }
}
