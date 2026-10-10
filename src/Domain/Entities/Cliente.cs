using Domain.Common.Entities;

namespace Domain.Entities;

public class Cliente: SoftDeleteBaseEntity
{
    public string Nome { get; private set; } = string.Empty;

    //public Documento Documento { get; private set; }
    public string CPFCNPJ { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string? Telefone { get; private set; }

    public string Endereco { get; private set; } = string.Empty;

    public ICollection<Veiculo> Veiculos { get; set; }  = [];

    public Cliente(string nome, string cpfcnpj, string email, string telefone, string endereco)
    {
        Nome = nome;
        CPFCNPJ = cpfcnpj;
        Email = email;
        Telefone = telefone;
        Endereco = endereco;
    }
}
