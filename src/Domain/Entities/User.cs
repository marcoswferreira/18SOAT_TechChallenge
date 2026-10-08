using Domain.Common.Entities;

namespace Domain.Entities;

public class User : SoftDeleteBaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public IList<UserRole> Roles { get; private set; } = [];

    public string? RefreshTokenHash { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    public User() { }

    public User(string email, string passwordHash, IEnumerable<string> roles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Roles = roles.Select(r => new UserRole(Id, r)).ToList();
    }

    public void Update(string email, IEnumerable<string> roles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Email = email.Trim().ToLowerInvariant();
        Roles = roles.Select(r => new UserRole(Id, r)).ToList();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
        PasswordHash = newPasswordHash;
    }

    public void AddRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        if (!HasRole(role))
            Roles.Add(new UserRole(Id, role));
    }

    public void RemoveRole(string role)
    {
        var existing = Roles.FirstOrDefault(r => r.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
            Roles.Remove(existing);
    }

    public bool HasRole(string role) =>
        Roles.Any(r => r.Role.Equals(role, StringComparison.OrdinalIgnoreCase));

    public void SetRefreshToken(string tokenHash, TimeSpan duration)
    {
        RefreshTokenHash = tokenHash;
        RefreshTokenExpiresAt = DateTime.UtcNow.Add(duration);
    }

    public bool IsRefreshTokenValid(string tokenHash)
    {
        return RefreshTokenHash == tokenHash
            && RefreshTokenExpiresAt.HasValue
            && RefreshTokenExpiresAt.Value > DateTime.UtcNow;
    }

    public void RevokeRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiresAt = null;
    }
}
