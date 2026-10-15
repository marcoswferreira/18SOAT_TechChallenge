using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial record Endereco : IValueObject
{
    private static readonly HashSet<string> EstadosValidos =
    [
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
        "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
        "RS", "RO", "RR", "SC", "SP", "SE", "TO"
    ];

    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex CepRegex();

    public string Logradouro { get; }
    public string Numero { get; }
    public string? Complemento { get; }
    public string Bairro { get; }
    public string Cidade { get; }
    public string Estado { get; }
    public string Cep { get; }

    public Endereco(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string estado,
        string cep)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logradouro);
        ArgumentException.ThrowIfNullOrWhiteSpace(numero);
        ArgumentException.ThrowIfNullOrWhiteSpace(bairro);
        ArgumentException.ThrowIfNullOrWhiteSpace(cidade);
        ArgumentException.ThrowIfNullOrWhiteSpace(estado);
        ArgumentException.ThrowIfNullOrWhiteSpace(cep);

        var estadoNormalizado = estado.Trim().ToUpperInvariant();
        if (!EstadosValidos.Contains(estadoNormalizado))
        {
            throw new ArgumentException($"Estado (UF) inválido: '{estado}'.", nameof(estado));
        }

        var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
        if (!CepRegex().IsMatch(cepLimpo))
        {
            throw new ArgumentException("O CEP deve conter exatamente 8 dígitos numéricos.", nameof(cep));
        }

        Logradouro = logradouro.Trim();
        Numero = numero.Trim();
        Complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento.Trim();
        Bairro = bairro.Trim();
        Cidade = cidade.Trim();
        Estado = estadoNormalizado;
        Cep = cepLimpo;
    }

    public string CepFormatado => $"{Cep[..5]}-{Cep[5..]}";

    public string Formatado =>
        string.IsNullOrWhiteSpace(Complemento)
            ? $"{Logradouro}, {Numero}, {Bairro}, {Cidade} - {Estado}, CEP: {CepFormatado}"
            : $"{Logradouro}, {Numero} - {Complemento}, {Bairro}, {Cidade} - {Estado}, CEP: {CepFormatado}";

    public override string ToString() => Formatado;

    public static bool EhEstadoValido(string? estado) =>
        !string.IsNullOrWhiteSpace(estado) && EstadosValidos.Contains(estado.Trim().ToUpperInvariant());

    public static bool EhCepValido(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep)) return false;
        var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
        return CepRegex().IsMatch(cepLimpo);
    }
}
