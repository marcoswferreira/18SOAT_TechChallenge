public sealed record Placa
{
    public string Valor {get;}

    public Placa(string valor)
    {
        if(string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("A placa é obrigatória.", nameof(valor));

        valor = valor.Trim().ToUpperInvariant();

        if(!PlacaValida(valor))
            throw new ArgumentException("A placa informada é inválida.", nameof(valor));

        Valor = valor;
    }

    private static bool PlacaValida(string valor)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(valor, @"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$");
    }
}