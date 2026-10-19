using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.OrdensServico.Dto;

public record ItemServicoOutput(
    Guid? ServicoId,
    string Descricao,
    decimal ValorUnitario,
    int Quantidade,
    decimal ValorTotal
)
{
    public static ItemServicoOutput FromValueObject(ItemServico s)
        => new(s.ServicoId, s.Descricao, s.ValorUnitario, s.Quantidade, s.ValorTotal);
}

public record ItemPecaOutput(
    Guid ItemEstoqueId,
    string NomePeca,
    decimal ValorUnitario,
    decimal Quantidade,
    decimal ValorTotal
)
{
    public static ItemPecaOutput FromValueObject(ItemPeca p)
        => new(p.ItemEstoqueId, p.NomePeca, p.ValorUnitario, p.Quantidade, p.ValorTotal);
}

public record RegistroStatusOutput(
    string? De,
    string Para,
    DateTime Em
)
{
    public static RegistroStatusOutput FromEntity(RegistroStatus r)
        => new(r.De?.ToString(), r.Para.ToString(), r.Em);
}

public record OrdemServicoOutput(
    Guid Id,
    Guid ClienteId,
    Guid VeiculoId,
    string Status,
    string StatusDescricao,
    decimal ValorTotal,
    IReadOnlyCollection<ItemServicoOutput> Servicos,
    IReadOnlyCollection<ItemPecaOutput> Pecas,
    IReadOnlyCollection<RegistroStatusOutput> Historico
)
{
    public static OrdemServicoOutput FromEntity(OrdemServico os)
    {
        return new OrdemServicoOutput(
            os.Id,
            os.ClienteId,
            os.VeiculoId,
            os.Status.ToString(),
            ObterDescricaoStatus(os.Status),
            os.ValorTotal,
            os.Servicos.Select(ItemServicoOutput.FromValueObject).ToList(),
            os.Pecas.Select(ItemPecaOutput.FromValueObject).ToList(),
            os.Historico.Select(RegistroStatusOutput.FromEntity).ToList()
        );
    }

    private static string ObterDescricaoStatus(StatusOrdemServico status) => status switch
    {
        StatusOrdemServico.Recebida => "Sua ordem de serviço foi recebida e aguarda início da avaliação.",
        StatusOrdemServico.EmDiagnostico => "Nossa equipe técnica está analisando o veículo.",
        StatusOrdemServico.AguardandoAprovacao => "Orçamento gerado! Aguardando aprovação.",
        StatusOrdemServico.EmExecucao => "Reparo autorizado e serviço em andamento.",
        StatusOrdemServico.Finalizada => "Serviço concluído! O veículo está pronto para retirada.",
        StatusOrdemServico.Entregue => "Veículo entregue ao cliente.",
        StatusOrdemServico.Cancelada => "Orçamento/OS cancelada.",
        _ => "Status desconhecido."
    };
}

