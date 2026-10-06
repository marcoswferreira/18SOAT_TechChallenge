using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Users.Dto;

public record UpdateUserInput(
    [EmailAddress] string Email,
    IList<string> Roles);
