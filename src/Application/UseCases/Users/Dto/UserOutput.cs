using Domain.Entities;

namespace Application.UseCases.Users.Dto;

public record UserOutput(
    Guid Id,
    string Email,
    IList<string> Roles,
    DateTime CreatedAt,
    DateTime? LastModifiedAt)
{
    public static UserOutput FromEntity(User user) => new(
        user.Id,
        user.Email,
        [.. user.Roles.Select(r => r.Role)],
        user.CreatedAt,
        user.LastModifiedAt);
}
