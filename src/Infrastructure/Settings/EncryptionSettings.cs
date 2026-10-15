using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Settings;

public class EncryptionSettings
{
    public const string SectionName = "EncryptionSettings";

    [Required(ErrorMessage = "A chave de criptografia de dados sensíveis (LGPD) é obrigatória.")]
    [MinLength(32, ErrorMessage = "A chave de criptografia deve ter no mínimo 32 caracteres (256 bits).")]
    public string Key { get; init; } = string.Empty;
}

