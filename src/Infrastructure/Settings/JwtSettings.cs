using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required(ErrorMessage = "O Secret do JWT é obrigatório.")]
    [MinLength(32, ErrorMessage = "O Secret do JWT deve ter no mínimo 32 caracteres (256 bits).")]
    public string Secret { get; init; } = string.Empty;

    [Required(ErrorMessage = "O Issuer é obrigatório.")]
    public string Issuer { get; init; } = string.Empty;

    [Required(ErrorMessage = "O Audience é obrigatório.")]
    public string Audience { get; init; } = string.Empty;

    [Range(1, 1440, ErrorMessage = "O tempo de expiração do Access Token deve ser entre 1 e 1440 minutos.")]
    public int AccessTokenExpirationMinutes { get; init; }

    [Range(1, 90, ErrorMessage = "O tempo de expiração do Refresh Token deve ser entre 1 e 90 dias.")]
    public int RefreshTokenExpirationDays { get; init; }
}