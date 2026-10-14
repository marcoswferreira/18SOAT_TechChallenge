using Domain.Entities;
using Domain.Enums;

namespace Application.Dto;

public record ConsultaStatusClienteResponse(
    Guid OrdemServicoId,
    string Status,
    string StatusDescricao,
    decimal ValorTotal,
    int TotalServicos,
    int TotalPecas
)
{
    public static ConsultaStatusClienteResponse FromDomain(OrdemServico os)
    {
        return new ConsultaStatusClienteResponse(
            os.Id,
            os.Status.ToString(),
            ObterDescricaoStatus(os.Status),
            os.ValorTotal,
            os.Servicos.Count,
            os.Pecas.Count
        );
    }

    private static string ObterDescricaoStatus(StatusOrdemServico status) => status switch
    {
        StatusOrdemServico.Recebida => "Sua ordem de serviço foi recebida e aguarda inicio da avaliação.",
        StatusOrdemServico.EmDiagnostico => "Nossa equipe técnica está analisando o veículo.",
        StatusOrdemServico.AguardandoAprovacao => "Orçamento gerado! Aguardando sua aprovação.",
        StatusOrdemServico.EmExecucao => "Reparo autorizado e serviço em andamento.",
        StatusOrdemServico.Finalizada => "Serviço concluído! O veículo está pronto para retirada.",
        StatusOrdemServico.Entregue => "Veículo entregue ao cliente.",
        StatusOrdemServico.Cancelada => "Orçamento/OS cancelada.",
        _ => "Status desconhecido."
    };
}
