namespace Application.UseCases.Users.Dto;

public record UpdateUserInput(
    string Email,
    IList<string> Roles);
