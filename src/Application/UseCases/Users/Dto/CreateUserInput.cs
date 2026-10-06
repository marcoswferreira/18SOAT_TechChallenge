namespace Application.UseCases.Users.Dto;

public record CreateUserInput(
    string Email,
    string Password,
    string Role);
