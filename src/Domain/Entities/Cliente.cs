using Domain.Common.Entities;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Cliente : SoftDeleteBaseEntity, IAggregateRoot
{
    private readonly List<Veiculo> _veiculos = [];

    public string Nome { get; private set; } = string.Empty;
    public Documento Documento { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }
    public Endereco Endereco { get; private set; } = null!;

    public virtual IReadOnlyCollection<Veiculo> Veiculos => _veiculos.AsReadOnly();

    protected Cliente() { }

    public Cliente(string nome, Documento documento, string? email, string? telefone, Endereco endereco)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentNullException.ThrowIfNull(documento);
        ArgumentNullException.ThrowIfNull(endereco);

        Nome = nome.Trim();
        Documento = documento;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
        Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim();
        Endereco = endereco;
    }

    public void Update(string nome, string? email, string? telefone, Endereco endereco)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentNullException.ThrowIfNull(endereco);

        Nome = nome.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
        Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim();
        Endereco = endereco;
    }

    public void UpdateDocumento(Documento novoDocumento)
    {
        ArgumentNullException.ThrowIfNull(novoDocumento);
        Documento = novoDocumento;
    }

    public void UpdateEndereco(Endereco novoEndereco)
    {
        ArgumentNullException.ThrowIfNull(novoEndereco);
        Endereco = novoEndereco;
    }

    public void AdicionarVeiculo(Veiculo veiculo)
    {
        ArgumentNullException.ThrowIfNull(veiculo);

        if (_veiculos.Any(v => v.Placa == veiculo.Placa || (v.Id != Guid.Empty && v.Id == veiculo.Id)))
        {
            throw new InvalidOperationException($"O veículo com a placa '{veiculo.Placa.Valor}' já está associado a este cliente.");
        }

        _veiculos.Add(veiculo);
    }

    public void RemoverVeiculo(Guid veiculoId)
    {
        var veiculo = _veiculos.FirstOrDefault(v => v.Id == veiculoId);
        if (veiculo is not null)
        {
            _veiculos.Remove(veiculo);
        }
    }
}
