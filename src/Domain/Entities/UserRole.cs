namespace Domain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }
    public string Role { get; private set; } = string.Empty;

    public User User { get; private set; } = null!;

    public UserRole() { }

    public UserRole(Guid userId, string role)
    {
        UserId = userId;
        Role = role;
    }
}
