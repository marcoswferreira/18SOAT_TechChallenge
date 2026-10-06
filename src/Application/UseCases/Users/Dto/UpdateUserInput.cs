namespace Application.UseCases.Users.Dto;

public record UpdateUserInput(
    string Email,
    string Role);
