using Domain.Constants;

namespace Domain.UnitTests.Constants;

public class RolesTests
{
    // -------------------------------------------------------------------------
    // Constants
    // -------------------------------------------------------------------------

    [Fact]
    public void Constants_ShouldHaveExpectedValues()
    {
        Assert.Equal("Admin", Roles.Admin);
        Assert.Equal("User", Roles.User);
        Assert.Equal("Manager", Roles.Manager);
        Assert.Equal("Atendente", Roles.Atendente);
    }

    [Fact]
    public void All_ShouldContainAllFourRoles()
    {
        Assert.Equal(4, Roles.All.Length);
        Assert.Contains(Roles.Admin, Roles.All);
        Assert.Contains(Roles.User, Roles.All);
        Assert.Contains(Roles.Manager, Roles.All);
        Assert.Contains(Roles.Atendente, Roles.All);
    }

    // -------------------------------------------------------------------------
    // IsValid — roles válidas
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData("Admin")]
    [InlineData("admin")]
    [InlineData("ADMIN")]
    [InlineData("User")]
    [InlineData("user")]
    [InlineData("Manager")]
    [InlineData("manager")]
    [InlineData("Atendente")]
    [InlineData("atendente")]
    [InlineData("ATENDENTE")]
    public void IsValid_WithValidRoles_ShouldReturnTrue(string role)
    {
        Assert.True(Roles.IsValid(role));
    }

    // -------------------------------------------------------------------------
    // IsValid — roles inválidas
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Guest")]
    [InlineData("Root")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void IsValid_WithInvalidRoles_ShouldReturnFalse(string? role)
    {
        Assert.False(Roles.IsValid(role!));
    }
}
