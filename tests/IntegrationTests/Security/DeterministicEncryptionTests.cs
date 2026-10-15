using Infrastructure.Security;
using Xunit;

namespace IntegrationTests.Security;

public class DeterministicEncryptionTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("11222333000181")]
    [InlineData("BRA2E19")]
    [InlineData("ABC1234")]
    public void Encrypt_WithSameInput_ShouldAlwaysReturnIdenticalCipherText(string input)
    {
        // Act
        var cipher1 = DeterministicEncryption.Encrypt(input);
        var cipher2 = DeterministicEncryption.Encrypt(input);

        // Assert
        Assert.NotNull(cipher1);
        Assert.NotEmpty(cipher1);
        Assert.Equal(cipher1, cipher2);
        Assert.NotEqual(input, cipher1);
    }

    [Theory]
    [InlineData("52998224725", "85369680070")]
    [InlineData("BRA2E19", "ABC1234")]
    public void Encrypt_WithDifferentInputs_ShouldProduceDifferentCipherTexts(string input1, string input2)
    {
        var cipher1 = DeterministicEncryption.Encrypt(input1);
        var cipher2 = DeterministicEncryption.Encrypt(input2);

        Assert.NotEqual(cipher1, cipher2);
    }

    [Theory]
    [InlineData("52998224725")]
    [InlineData("11222333000181")]
    [InlineData("BRA2E19")]
    [InlineData("ABC1234")]
    public void Decrypt_WithEncryptedString_ShouldReturnOriginalPlainText(string original)
    {
        var encrypted = DeterministicEncryption.Encrypt(original);
        var decrypted = DeterministicEncryption.Decrypt(encrypted);

        Assert.Equal(original, decrypted);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Encrypt_WithNullOrEmpty_ShouldReturnEmpty(string? input)
    {
        var result = DeterministicEncryption.Encrypt(input);
        Assert.True(string.IsNullOrEmpty(result));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Decrypt_WithNullOrEmpty_ShouldReturnEmpty(string? input)
    {
        var result = DeterministicEncryption.Decrypt(input);
        Assert.True(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void Decrypt_WithUnencryptedPlainText_ShouldReturnOriginalGracefully()
    {
        const string unencrypted = "52998224725";
        var result = DeterministicEncryption.Decrypt(unencrypted);

        Assert.Equal(unencrypted, result);
    }
}

