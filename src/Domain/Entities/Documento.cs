namespace Domain.Entities;

public sealed record Documento
{
    public TipoDocumento Tipo { get; }
    public string Valor { get; }

    public Documento(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException( "O documento é obrigatório.", nameof(valor));

        valor = new string(valor.Where(char.IsDigit).ToArray());

        Tipo = valor.Length switch
        {
            11 when ValidarCpf(valor) => TipoDocumento.CPF,
            14 when ValidarCnpj(valor) => TipoDocumento.CNPJ,
            _ => throw new ArgumentException("CPF ou CNPJ inválido.", nameof(valor))
        };

        Valor = valor;
    }

    private static bool ValidarCpf(string numero)
    {
        // Implementar a validação dos dígitos verificadores.
        throw new NotImplementedException();
    }

    private static bool ValidarCnpj(string numero)
    {
        // Implementar a validação dos dígitos verificadores.
        throw new NotImplementedException();
    }
}