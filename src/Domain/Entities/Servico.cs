using Domain.Common.Entities;

namespace Domain.Entities;

public class Servico : SoftDeleteBaseEntity
{
    public string Nome { get; private set; } = string.Empty;

    protected Servico() { }

    public Servico(string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        Nome = nome.Trim();
    }

    public Servico(Guid id, string nome) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        Nome = nome.Trim();
    }

    public void Update(string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        Nome = nome.Trim();
    }
}
