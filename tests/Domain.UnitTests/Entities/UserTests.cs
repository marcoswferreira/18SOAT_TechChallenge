using Domain.Constants;
using Domain.Entities;
using Xunit;

namespace Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateUserAndNormalizeEmail()
    {
        // Arrange
        var email = "  USER.Test@Example.COM  ";
        var passwordHash = "hashed_secret_password";
        var role = Roles.Admin;

        // Act
        var user = new User(email, passwordHash, role);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("user.test@example.com", user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(role, user.Role);
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.Null(user.DeletedBy);
    }

    [Theory]
    [InlineData("", "password", "Admin")]
    [InlineData("   ", "password", "Admin")]
    [InlineData("test@example.com", "", "Admin")]
    [InlineData("test@example.com", "password", "")]
    public void Constructor_WithInvalidParameters_ShouldThrowArgumentException(string email, string passwordHash, string role)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new User(email, passwordHash, role));
    }

    [Fact]
    public void Update_WithValidInput_ShouldUpdateEmailAndRole()
    {
        // Arrange
        var user = new User("original@example.com", "hash", Roles.User);

        // Act
        user.Update("  UPDATED@EXAMPLE.COM  ", Roles.Manager);

        // Assert
        Assert.Equal("updated@example.com", user.Email);
        Assert.Equal(Roles.Manager, user.Role);
    }

    [Theory]
    [InlineData("", "Admin")]
    [InlineData("valid@example.com", "")]
    public void Update_WithInvalidInput_ShouldThrowArgumentException(string email, string role)
    {
        // Arrange
        var user = new User("original@example.com", "hash", Roles.User);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Update(email, role));
    }

    [Fact]
    public void UpdatePassword_WithValidHash_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = new User("user@example.com", "old_hash", Roles.User);

        // Act
        user.UpdatePassword("new_secure_hash");

        // Assert
        Assert.Equal("new_secure_hash", user.PasswordHash);
    }

    [Fact]
    public void SetRefreshToken_ShouldStoreHashAndExpiration()
    {
        // Arrange
        var user = new User("user@example.com", "hash", Roles.User);
        var tokenHash = "token_hash_123";
        var duration = TimeSpan.FromHours(1);

        // Act
        user.SetRefreshToken(tokenHash, duration);

        // Assert
        Assert.Equal(tokenHash, user.RefreshTokenHash);
        Assert.NotNull(user.RefreshTokenExpiresAt);
        Assert.True(user.RefreshTokenExpiresAt > DateTime.UtcNow);
        Assert.True(user.IsRefreshTokenValid(tokenHash));
    }

    [Fact]
    public void IsRefreshTokenValid_WithExpiredOrMismatchToken_ShouldReturnFalse()
    {
        // Arrange
        var user = new User("user@example.com", "hash", Roles.User);
        var tokenHash = "token_hash_123";
        user.SetRefreshToken(tokenHash, TimeSpan.FromHours(-1)); // expired 1 hour ago

        // Act & Assert
        Assert.False(user.IsRefreshTokenValid(tokenHash));
        Assert.False(user.IsRefreshTokenValid("different_hash"));
    }

    [Fact]
    public void RevokeRefreshToken_ShouldClearTokenData()
    {
        // Arrange
        var user = new User("user@example.com", "hash", Roles.User);
        user.SetRefreshToken("token_hash_123", TimeSpan.FromHours(1));

        // Act
        user.RevokeRefreshToken();

        // Assert
        Assert.Null(user.RefreshTokenHash);
        Assert.Null(user.RefreshTokenExpiresAt);
        Assert.False(user.IsRefreshTokenValid("token_hash_123"));
    }

    [Fact]
    public void Delete_ShouldSetSoftDeleteProperties()
    {
        // Arrange
        var user = new User("user@example.com", "hash", Roles.User);
        var deletedBy = "admin-user-id";

        // Act
        user.Delete(deletedBy);

        // Assert
        Assert.True(user.IsDeleted);
        Assert.NotNull(user.DeletedAt);
        Assert.Equal(deletedBy, user.DeletedBy);
    }

    [Fact]
    public void Restore_ShouldClearSoftDeleteProperties()
    {
        // Arrange
        var user = new User("user@example.com", "hash", Roles.User);
        user.Delete("admin-id");

        // Act
        user.Restore();

        // Assert
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.Null(user.DeletedBy);
    }
}
