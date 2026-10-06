namespace Application.UseCases.Users.Dto;

public record CreateUserInput(
    string Email,
    string Password,
    IList<string> Roles);
