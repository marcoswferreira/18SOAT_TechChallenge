using Domain.Common.Entities;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Veiculo : SoftDeleteBaseEntity
{
    public Guid ClienteId { get; private set; }
    public Placa Placa { get; private set; } = null!;
    public string Marca { get; private set; } = string.Empty;
    public string Modelo { get; private set; } = string.Empty;
    public int Ano { get; private set; }

    // Propriedade de navegação
    public virtual Cliente? Cliente { get; private set; }

    protected Veiculo() { }

    public Veiculo(Guid clienteId, Placa placa, string marca, string modelo, int ano)
    {
        if (clienteId == Guid.Empty)
            throw new ArgumentException("O identificador do cliente é obrigatório.", nameof(clienteId));

        ArgumentNullException.ThrowIfNull(placa);
        ArgumentException.ThrowIfNullOrWhiteSpace(marca);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelo);

        var limiteAnoMaximo = DateTime.UtcNow.Year + 1;
        if (ano < 1900 || ano > limiteAnoMaximo)
            throw new ArgumentOutOfRangeException(nameof(ano), $"O ano do veículo deve ser entre 1900 e {limiteAnoMaximo}.");

        ClienteId = clienteId;
        Placa = placa;
        Marca = marca.Trim();
        Modelo = modelo.Trim();
        Ano = ano;
    }

    public void Update(Placa placa, string marca, string modelo, int ano)
    {
        ArgumentNullException.ThrowIfNull(placa);
        ArgumentException.ThrowIfNullOrWhiteSpace(marca);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelo);

        var limiteAnoMaximo = DateTime.UtcNow.Year + 1;
        if (ano < 1900 || ano > limiteAnoMaximo)
            throw new ArgumentOutOfRangeException(nameof(ano), $"O ano do veículo deve ser entre 1900 e {limiteAnoMaximo}.");

        Placa = placa;
        Marca = marca.Trim();
        Modelo = modelo.Trim();
        Ano = ano;
    }

    public void TransferirParaCliente(Guid novoClienteId)
    {
        if (novoClienteId == Guid.Empty)
            throw new ArgumentException("O novo identificador do cliente é obrigatório.", nameof(novoClienteId));

        ClienteId = novoClienteId;
        Cliente = null;
    }
}
