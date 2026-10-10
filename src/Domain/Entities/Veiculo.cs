using Domain.Common.Entities;

namespace Domain.Entities;

public class Veiculo : SoftDeleteBaseEntity
{
    public Guid ClienteId { get; private set; }

    public Placa Placa { get; private set; }

    public string Marca { get; private set; } = string.Empty;

    public string Modelo { get; private set; } = string.Empty;

    public int Ano { get; private set; }

    // Propriedade de navegação
    public Cliente Cliente { get; set; } = null!;

    public Veiculo(Guid clienteId, Placa placa, int ano)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ano);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(ano, DateTime.UtcNow.Year);

        ClienteId = clienteId;
        Placa = placa;
        Ano = ano;

    }
}
