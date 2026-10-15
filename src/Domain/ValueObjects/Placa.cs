using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial record Placa : IValueObject
{
    [GeneratedRegex(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")]
    private static partial Regex PlacaRegex();

    public string Valor { get; }

    public Placa(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("A placa é obrigatória.", nameof(valor));

        var limpa = valor.Trim().Replace("-", "").ToUpperInvariant();

        if (!PlacaValida(limpa))
            throw new ArgumentException("A placa informada é inválida.", nameof(valor));

        Valor = limpa;
    }

    public string Formatado => EhMercosul ? Valor : $"{Valor[..3]}-{Valor[3..]}";

    public bool EhMercosul => char.IsLetter(Valor[4]);

    /// <summary>
    /// Retorna a placa mascarada para proteção de dados sensíveis em conformidade com a LGPD.
    /// Exemplo: BRA***9 ou ABC***4.
    /// </summary>
    public string Mascarada => $"{Valor[..3]}***{Valor[6..]}";

    public override string ToString() => Valor;

    public static implicit operator string(Placa placa) => placa.Valor;

    public static bool EhValida(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        var limpa = valor.Trim().Replace("-", "").ToUpperInvariant();
        return PlacaRegex().IsMatch(limpa);
    }

    private static bool PlacaValida(string valor) => PlacaRegex().IsMatch(valor);
}
