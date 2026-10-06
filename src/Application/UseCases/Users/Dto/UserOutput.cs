using Domain.Entities;

namespace Application.UseCases.Users.Dto;

public record UserOutput(
    Guid Id,
    string Email,
    string Role,
    DateTime CreatedAt,
    DateTime? LastModifiedAt)
{
    public static UserOutput FromEntity(User user) => new(
        user.Id,
        user.Email,
        user.Role,
        user.CreatedAt,
        user.LastModifiedAt);
}
