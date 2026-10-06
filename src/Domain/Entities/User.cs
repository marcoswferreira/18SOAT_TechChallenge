using Domain.Common.Entities;
using Domain.Constants;

namespace Domain.Entities;

public class User : SoftDeleteBaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;

    public string? RefreshTokenHash { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    public User() { }

    public User(string email, string passwordHash, string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
    }

    public void Update(string email, string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        Email = email.Trim().ToLowerInvariant();
        Role = role;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
        PasswordHash = newPasswordHash;
    }

    public void UpdateRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        Role = role;
    }

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
