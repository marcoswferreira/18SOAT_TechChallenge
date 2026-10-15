using Domain.Enums;

namespace Domain.ValueObjects;

public sealed record Documento : IValueObject
{
    private static readonly int[] MultiplicadoresCpf1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] MultiplicadoresCpf2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

    private static readonly int[] MultiplicadoresCnpj1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] MultiplicadoresCnpj2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    public TipoDocumento Tipo { get; }
    public string Valor { get; }

    public Documento(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O documento é obrigatório.", nameof(valor));

        var somenteNumeros = new string(valor.Where(char.IsDigit).ToArray());

        Tipo = somenteNumeros.Length switch
        {
            11 when ValidarCpf(somenteNumeros) => TipoDocumento.CPF,
            14 when ValidarCnpj(somenteNumeros) => TipoDocumento.CNPJ,
            _ => throw new ArgumentException("CPF ou CNPJ inválido.", nameof(valor))
        };

        Valor = somenteNumeros;
    }

    public string Formatado => Tipo switch
    {
        TipoDocumento.CPF => Convert.ToUInt64(Valor).ToString(@"000\.000\.000\-00"),
        TipoDocumento.CNPJ => Convert.ToUInt64(Valor).ToString(@"00\.000\.000\/0000\-00"),
        _ => Valor
    };

    public override string ToString() => Valor;

    public static implicit operator string(Documento documento) => documento.Valor;

    public static bool EhValido(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        var somenteNumeros = new string(valor.Where(char.IsDigit).ToArray());
        return somenteNumeros.Length switch
        {
            11 => ValidarCpf(somenteNumeros),
            14 => ValidarCnpj(somenteNumeros),
            _ => false
        };
    }

    public static bool ValidarCpf(string numero)
    {
        if (numero.Length != 11 || numero.Distinct().Count() == 1)
            return false;

        var soma1 = 0;
        for (var i = 0; i < 9; i++)
            soma1 += (numero[i] - '0') * MultiplicadoresCpf1[i];

        var resto1 = soma1 % 11;
        var digito1 = resto1 < 2 ? 0 : 11 - resto1;

        if (numero[9] - '0' != digito1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 10; i++)
            soma2 += (numero[i] - '0') * MultiplicadoresCpf2[i];

        var resto2 = soma2 % 11;
        var digito2 = resto2 < 2 ? 0 : 11 - resto2;

        return numero[10] - '0' == digito2;
    }

    public static bool ValidarCnpj(string numero)
    {
        if (numero.Length != 14 || numero.Distinct().Count() == 1)
            return false;

        var soma1 = 0;
        for (var i = 0; i < 12; i++)
            soma1 += (numero[i] - '0') * MultiplicadoresCnpj1[i];

        var resto1 = soma1 % 11;
        var digito1 = resto1 < 2 ? 0 : 11 - resto1;

        if (numero[12] - '0' != digito1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 13; i++)
            soma2 += (numero[i] - '0') * MultiplicadoresCnpj2[i];

        var resto2 = soma2 % 11;
        var digito2 = resto2 < 2 ? 0 : 11 - resto2;

        return numero[13] - '0' == digito2;
    }
}

