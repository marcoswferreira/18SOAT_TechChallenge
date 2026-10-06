using Domain.Constants;
using Xunit;

namespace Domain.UnitTests.Constants;

public class RolesTests
{
    [Theory]
    [InlineData("Admin")]
    [InlineData("admin")]
    [InlineData("User")]
    [InlineData("user")]
    [InlineData("Manager")]
    [InlineData("manager")]
    public void IsValid_WithValidRoles_ShouldReturnTrue(string role)
    {
        // Act
        var result = Roles.IsValid(role);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Guest")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void IsValid_WithInvalidRoles_ShouldReturnFalse(string? role)
    {
        // Act
        var result = Roles.IsValid(role!);

        // Assert
        Assert.False(result);
    }
}
