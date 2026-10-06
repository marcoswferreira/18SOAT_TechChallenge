namespace Application.UseCases.Users.Dto;

public record ChangePasswordInput(
    string CurrentPassword,
    string NewPassword);
