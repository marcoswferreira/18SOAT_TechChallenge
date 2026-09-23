using Domain.Common.Entities;

namespace Domain.Entities;

public class User : SoftDeleteBaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;

    public string? RefreshTokenHash { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

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
