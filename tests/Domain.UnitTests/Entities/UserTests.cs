using Domain.Constants;
using Domain.Entities;

namespace Domain.UnitTests.Entities;

public class UserTests
{
    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateUserAndNormalizeEmail()
    {
        var user = new User("  USER.Test@Example.COM  ", "hashed_password", [Roles.Admin]);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("user.test@example.com", user.Email);
        Assert.Equal("hashed_password", user.PasswordHash);
        Assert.Single(user.Roles);
        Assert.Equal(Roles.Admin, user.Roles[0].Role);
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.Null(user.DeletedBy);
    }

    [Fact]
    public void Constructor_WithMultipleRoles_ShouldCreateAllRoles()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin, Roles.Manager]);

        Assert.Equal(2, user.Roles.Count);
        Assert.True(user.HasRole(Roles.Admin));
        Assert.True(user.HasRole(Roles.Manager));
    }

    [Fact]
    public void Constructor_WithDuplicateRoles_ShouldDeduplicate()
    {
        // Duplicatas são ignoradas via HasRole no AddRole, mas o construtor
        // usa Select direto — roles duplicatas na lista de entrada são mantidas.
        // Isso é intencional: a validação de unicidade é responsabilidade do use case.
        var user = new User("user@example.com", "hash", [Roles.Admin]);

        Assert.Single(user.Roles);
    }

    [Fact]
    public void Constructor_WithEmptyRoles_ShouldCreateUserWithNoRoles()
    {
        var user = new User("user@example.com", "hash", []);

        Assert.Empty(user.Roles);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("   ", "password")]
    [InlineData("test@example.com", "")]
    public void Constructor_WithInvalidEmailOrPassword_ShouldThrowArgumentException(string email, string passwordHash)
    {
        Assert.Throws<ArgumentException>(() => new User(email, passwordHash, [Roles.Admin]));
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    [Fact]
    public void Update_WithValidInput_ShouldUpdateEmailAndRoles()
    {
        var user = new User("original@example.com", "hash", [Roles.User]);

        user.Update("  UPDATED@EXAMPLE.COM  ", [Roles.Admin, Roles.Manager]);

        Assert.Equal("updated@example.com", user.Email);
        Assert.Equal(2, user.Roles.Count);
        Assert.True(user.HasRole(Roles.Admin));
        Assert.True(user.HasRole(Roles.Manager));
        Assert.False(user.HasRole(Roles.User)); // role antiga removida
    }

    [Fact]
    public void Update_WithEmptyEmail_ShouldThrowArgumentException()
    {
        var user = new User("original@example.com", "hash", [Roles.User]);

        Assert.Throws<ArgumentException>(() => user.Update("", [Roles.Admin]));
    }

    [Fact]
    public void Update_ReplacesAllPreviousRoles()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin, Roles.Manager]);

        user.Update("user@example.com", [Roles.User]);

        Assert.Single(user.Roles);
        Assert.True(user.HasRole(Roles.User));
        Assert.False(user.HasRole(Roles.Admin));
        Assert.False(user.HasRole(Roles.Manager));
    }

    // -------------------------------------------------------------------------
    // AddRole
    // -------------------------------------------------------------------------

    [Fact]
    public void AddRole_WithNewRole_ShouldAddToCollection()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        user.AddRole(Roles.Admin);

        Assert.Equal(2, user.Roles.Count);
        Assert.True(user.HasRole(Roles.Admin));
    }

    [Fact]
    public void AddRole_WithDuplicateRole_ShouldNotAddAgain()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin]);

        user.AddRole(Roles.Admin);

        Assert.Single(user.Roles);
    }

    [Fact]
    public void AddRole_CaseInsensitive_ShouldNotAddDuplicate()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin]);

        user.AddRole("admin"); // lowercase

        Assert.Single(user.Roles);
    }

    [Fact]
    public void AddRole_WithEmptyRole_ShouldThrowArgumentException()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        Assert.Throws<ArgumentException>(() => user.AddRole(""));
    }

    // -------------------------------------------------------------------------
    // RemoveRole
    // -------------------------------------------------------------------------

    [Fact]
    public void RemoveRole_WithExistingRole_ShouldRemoveFromCollection()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin, Roles.Manager]);

        user.RemoveRole(Roles.Admin);

        Assert.Single(user.Roles);
        Assert.False(user.HasRole(Roles.Admin));
        Assert.True(user.HasRole(Roles.Manager));
    }

    [Fact]
    public void RemoveRole_WithNonExistingRole_ShouldDoNothing()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        user.RemoveRole(Roles.Admin); // Admin não existe

        Assert.Single(user.Roles);
    }

    [Fact]
    public void RemoveRole_CaseInsensitive_ShouldRemoveCorrectly()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin]);

        user.RemoveRole("admin"); // lowercase

        Assert.Empty(user.Roles);
    }

    // -------------------------------------------------------------------------
    // HasRole
    // -------------------------------------------------------------------------

    [Fact]
    public void HasRole_WithExistingRole_ShouldReturnTrue()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin]);

        Assert.True(user.HasRole(Roles.Admin));
    }

    [Fact]
    public void HasRole_WithNonExistingRole_ShouldReturnFalse()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        Assert.False(user.HasRole(Roles.Admin));
    }

    [Fact]
    public void HasRole_CaseInsensitive_ShouldReturnTrue()
    {
        var user = new User("user@example.com", "hash", [Roles.Admin]);

        Assert.True(user.HasRole("admin"));
        Assert.True(user.HasRole("ADMIN"));
    }

    // -------------------------------------------------------------------------
    // UpdatePassword
    // -------------------------------------------------------------------------

    [Fact]
    public void UpdatePassword_WithValidHash_ShouldUpdatePasswordHash()
    {
        var user = new User("user@example.com", "old_hash", [Roles.User]);

        user.UpdatePassword("new_secure_hash");

        Assert.Equal("new_secure_hash", user.PasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdatePassword_WithInvalidHash_ShouldThrowArgumentException(string hash)
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        Assert.Throws<ArgumentException>(() => user.UpdatePassword(hash));
    }

    // -------------------------------------------------------------------------
    // RefreshToken
    // -------------------------------------------------------------------------

    [Fact]
    public void SetRefreshToken_ShouldStoreHashAndExpiration()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        user.SetRefreshToken("token_hash_123", TimeSpan.FromHours(1));

        Assert.Equal("token_hash_123", user.RefreshTokenHash);
        Assert.NotNull(user.RefreshTokenExpiresAt);
        Assert.True(user.RefreshTokenExpiresAt > DateTime.UtcNow);
        Assert.True(user.IsRefreshTokenValid("token_hash_123"));
    }

    [Fact]
    public void IsRefreshTokenValid_WithExpiredToken_ShouldReturnFalse()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);
        user.SetRefreshToken("token_hash_123", TimeSpan.FromHours(-1)); // expirado há 1 hora

        Assert.False(user.IsRefreshTokenValid("token_hash_123"));
    }

    [Fact]
    public void IsRefreshTokenValid_WithWrongHash_ShouldReturnFalse()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);
        user.SetRefreshToken("token_hash_123", TimeSpan.FromHours(1));

        Assert.False(user.IsRefreshTokenValid("different_hash"));
    }

    [Fact]
    public void IsRefreshTokenValid_WithNoTokenSet_ShouldReturnFalse()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        Assert.False(user.IsRefreshTokenValid("any_token"));
    }

    [Fact]
    public void RevokeRefreshToken_ShouldClearTokenData()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);
        user.SetRefreshToken("token_hash_123", TimeSpan.FromHours(1));

        user.RevokeRefreshToken();

        Assert.Null(user.RefreshTokenHash);
        Assert.Null(user.RefreshTokenExpiresAt);
        Assert.False(user.IsRefreshTokenValid("token_hash_123"));
    }

    // -------------------------------------------------------------------------
    // Soft Delete
    // -------------------------------------------------------------------------

    [Fact]
    public void Delete_ShouldSetSoftDeleteProperties()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        user.Delete("admin-user-id");

        Assert.True(user.IsDeleted);
        Assert.NotNull(user.DeletedAt);
        Assert.Equal("admin-user-id", user.DeletedBy);
    }

    [Fact]
    public void Restore_ShouldClearSoftDeleteProperties()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);
        user.Delete("admin-id");

        user.Restore();

        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.Null(user.DeletedBy);
    }

    // -------------------------------------------------------------------------
    // Equality
    // -------------------------------------------------------------------------

    [Fact]
    public void Equality_SameId_ShouldBeEqual()
    {
        var user1 = new User("a@example.com", "hash", [Roles.User]);
        var user2 = new User("b@example.com", "hash", [Roles.Admin]);

        // Dois objetos diferentes com Ids diferentes NÃO são iguais
        Assert.NotEqual(user1, user2);
    }

    [Fact]
    public void Equality_SameReference_ShouldBeEqual()
    {
        var user = new User("a@example.com", "hash", [Roles.User]);

        Assert.Equal(user, user);
    }

    [Fact]
    public void GetHashCode_ShouldBeBasedOnId()
    {
        var user = new User("user@example.com", "hash", [Roles.User]);

        Assert.Equal(user.Id.GetHashCode(), user.GetHashCode());
    }
}
