using Domain.Enums;

namespace Domain.Entities;

public class RegistroStatus
{
    private RegistroStatus() { } 

    internal RegistroStatus(Guid ordemServicoId, StatusOrdemServico? de,
                             StatusOrdemServico para, DateTime em)
    {
        Id = Guid.NewGuid();
        OrdemServicoId = ordemServicoId;
        De = de;
        Para = para;
        Em = em;
    }

    public Guid Id { get; private set; }
    public Guid OrdemServicoId { get; private set; }
    public StatusOrdemServico? De { get; private set; } 
    public StatusOrdemServico Para { get; private set; }
    public DateTime Em { get; private set; } 
}