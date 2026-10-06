using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Users.Dto;

public record CreateUserInput(
    [EmailAddress] string Email,
    string Password,
    IList<string> Roles);
