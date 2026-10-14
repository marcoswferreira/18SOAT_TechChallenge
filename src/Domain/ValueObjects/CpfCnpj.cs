using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record CpfCnpj
{
    public string Numero { get; private set;  }

    public CpfCnpj(string numero)
    {
        var limpo = Regex.Replace(numero ?? string.Empty, @"[^\d]", "");
        if (limpo.Length != 11 && limpo.Length != 14)
            throw new ArgumentException("CPF ou CNPJ inválido. Deve conter 11 dígitos (CPF) ou 14 dígitos (CNPJ).");

        // Aqui podem ser adicionados os algoritmos de validação de dígitos verificadores
        Numero = limpo;
    }
}
