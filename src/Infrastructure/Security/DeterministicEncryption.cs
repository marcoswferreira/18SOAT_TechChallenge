using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

/// <summary>
/// Provedor de criptografia simétrica determinística (AES-256) para proteção
/// de dados sensíveis em repouso na base de dados (em conformidade com a LGPD).
/// Garante que o mesmo valor em texto plano sempre produza o mesmo texto cifrado,
/// viabilizando buscas exatas por índice no banco de dados relacional.
/// </summary>
public static class DeterministicEncryption
{
    private static readonly byte[] DefaultKey = SHA256.HashData(Encoding.UTF8.GetBytes("SIAES_LGPD_ENCRYPTION_SECRET_KEY_2026"));
    private static readonly byte[] DefaultIv = SHA256.HashData(Encoding.UTF8.GetBytes("SIAES_LGPD_DETERMINISTIC_IV_SALT"))[..16];

    private static byte[] _key = DefaultKey;
    private static byte[] _iv = DefaultIv;

    public static void Initialize(string? secretKey)
    {
        if (string.IsNullOrWhiteSpace(secretKey)) return;
        _key = SHA256.HashData(Encoding.UTF8.GetBytes(secretKey));
        _iv = SHA256.HashData(Encoding.UTF8.GetBytes(secretKey + "_IV_SALT"))[..16];
    }

    public static string Encrypt(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText ?? string.Empty;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(cipherBytes);
    }

    public static string Decrypt(string? cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText ?? string.Empty;

        try
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch
        {
            // Fallback caso receba dado não criptografado (compatibilidade com dados pré-existentes)
            return cipherText;
        }
    }
}

