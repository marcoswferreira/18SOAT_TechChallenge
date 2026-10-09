using Domain.Common.Entities;

namespace Domain.Entities;

public class Cliente
{
    public Guid Id  { get; private set;  }
    public string CPFCNPJ { get; private set; } = string.Empty; //Value Objects ???

    public Cliente() { }

}
