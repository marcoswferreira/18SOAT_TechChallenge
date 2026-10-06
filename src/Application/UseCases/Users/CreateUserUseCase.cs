using Application.UseCases.Users.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.Users;

public class CreateUserUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<UserOutput> ExecuteAsync(CreateUserInput input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password))
        {
            throw new ArgumentException("E-mail e senha são obrigatórios.");
        }

        var role = string.IsNullOrWhiteSpace(input.Role) ? Roles.User : input.Role.Trim();

        if (!Roles.IsValid(role))
        {
            throw new ArgumentException($"Role inválida. As roles permitidas são: {string.Join(", ", Roles.All)}.");
        }

        var emailExists = await _userRepository.ExistsByEmailAsync(input.Email, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail '{input.Email}'.");
        }

        var passwordHash = _passwordHasher.Hash(input.Password);
        var user = new User(input.Email, passwordHash, role);

        await _userRepository.AddAsync(user, cancellationToken);

        return UserOutput.FromEntity(user);
    }
}
