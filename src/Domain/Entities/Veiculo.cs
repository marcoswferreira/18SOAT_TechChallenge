using Domain.Common.Entities;

namespace Domain.Entities;

public class Veiculo : SoftDeleteBaseEntity
{

    public Guid ClienteId { get; private set; }

    public string Placa { get; private set; } //Value Objects ???

    public string Marca { get; private set; }

    public string Modelo { get; private set; }

    public int Ano { get; private set; }
}
