using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Users.Dto;

/// <summary>
/// Dados para autocadastro de novo usuário. O perfil atribuído é sempre 'User'.
/// </summary>
public record CreateUserInput(
    [EmailAddress] string Email,
    string Password);
