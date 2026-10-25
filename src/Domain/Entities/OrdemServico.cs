using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class OrdemServico
{
    private readonly List<ItemServico> _servicos = new();
    private readonly List<ItemPeca> _pecas = new();
    private readonly List<RegistroStatus> _historico = new();

    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public Guid VeiculoId { get; private set; }

    public Cliente? Cliente { get; private set; } 
    public Veiculo? Veiculo { get; private set; } 

    public StatusOrdemServico Status { get; private set; }
    public decimal ValorTotal { get; private set; }

    public IReadOnlyCollection<ItemServico> Servicos => _servicos.AsReadOnly();
    public IReadOnlyCollection<ItemPeca> Pecas => _pecas.AsReadOnly();
    public IReadOnlyCollection<RegistroStatus> Historico => _historico.AsReadOnly();

    private OrdemServico() { } 

    public OrdemServico(Guid clienteId, Guid veiculoId)
    {
        if (clienteId == Guid.Empty) throw new DomainException("Cliente é obrigatório.");
        if (veiculoId == Guid.Empty) throw new DomainException("Veículo é obrigatório.");

        Id = Guid.NewGuid();
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        Status = StatusOrdemServico.Recebida;

        // Adiciona primeiro registro do histórico: criação da OS
        _historico.Add(new RegistroStatus(Id, null, Status, DateTime.UtcNow));
    }

    // Orçamento

    public void AdicionarServico(ItemServico servico)
    {
        ArgumentNullException.ThrowIfNull(servico);
        _servicos.Add(servico);
        RecalcularOrcamento();
    }

    public void AdicionarServico(Servico servico, decimal valorUnitario, int quantidade = 1)
    {
        var itemServico = ItemServico.Criar(servico, valorUnitario, quantidade);
        AdicionarServico(itemServico);
    }

    public void AdicionarPeca(ItemPeca peca)
    {
        ArgumentNullException.ThrowIfNull(peca);
        _pecas.Add(peca);
        RecalcularOrcamento();
    }

    public void AdicionarPeca(ItemEstoque itemEstoque, decimal quantidade, decimal? valorUnitario = null)
    {
        var peca = ItemPeca.Criar(itemEstoque, quantidade, valorUnitario);
        AdicionarPeca(peca);
    }

    private void RecalcularOrcamento()
        => ValorTotal = _servicos.Sum(s => s.ValorTotal) + _pecas.Sum(p => p.ValorTotal);

    // Ciclo de vida 

    public void IniciarDiagnostico() => MudarStatus(StatusOrdemServico.EmDiagnostico);

    public void GerarEEnviarOrcamento()
    {
        if (ValorTotal <= 0)
            throw new DomainException(
                "Não é possível gerar um orçamento sem serviços ou peças.");

        MudarStatus(StatusOrdemServico.AguardandoAprovacao);
    }

    /// <summary>Registra a aprovação do cliente</summary>
    public void AprovarOrcamento()
    {
        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new DomainException(
                $"Não é possível aprovar o orçamento com a OS em '{Status}'.");
    }

    /// <summary>Rejeita o orçamento inicial: cancela a OS.</summary>
    public void RejeitarOrcamento()
    {
        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new DomainException(
                $"Não é possível rejeitar o orçamento com a OS em '{Status}'.");

        MudarStatus(StatusOrdemServico.Cancelada);
    }

    /// <summary>Orçamento aprovado: inicia a execução do serviço.</summary>
    public void ExecutarServico()
    {
        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new DomainException(
                $"Não é possível executar o serviço com a OS em '{Status}'.");

        MudarStatus(StatusOrdemServico.EmExecucao);
    }

    /// <summary>Item adicional encontrado durante a execução: pede nova aprovação.</summary>
    public void SolicitarAprovacaoAdicional()
    {
        if (Status != StatusOrdemServico.EmExecucao)
            throw new DomainException(
                $"Não é possível solicitar aprovação adicional com a OS em '{Status}'.");

        MudarStatus(StatusOrdemServico.AguardandoAprovacao);
    }

    /// <summary>Cliente aprova o adicional: retoma a execução.</summary>
    public void AprovarOrcamentoAdicional()
    {
        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new DomainException(
                $"Não é possível aprovar o orçamento adicional com a OS em '{Status}'.");

        MudarStatus(StatusOrdemServico.EmExecucao);
    }

    /// <summary>
    /// Cliente rejeita o adicional: a OS fica em AguardandoAprovacao.
    /// </summary>
    public void RejeitarOrcamentoAdicional()
    {
        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new DomainException(
                $"Não é possível rejeitar o orçamento adicional com a OS em '{Status}'.");

       // Hotspot em aberto — o atendente decide: RejeitarOs() cancela,
       // ou ExecutarServico() retoma só com o que já foi aprovado.
    }

    /// <summary>Atendente cancela a OS que está aguardando decisão.</summary>
    public void RejeitarOs()
    {
        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new DomainException(
                $"Não é possível rejeitar a OS em '{Status}'.");

        MudarStatus(StatusOrdemServico.Cancelada);
    }

    public void FinalizarServico() => MudarStatus(StatusOrdemServico.Finalizada);
    public void RegistrarEntrega() => MudarStatus(StatusOrdemServico.Entregue);

    public TimeSpan? TempoDeExecucao()
    {
        var ordenado = _historico.OrderBy(r => r.Em).ToList();

        if (!ordenado.Any(r => r.Para == StatusOrdemServico.Finalizada))
            return null;

        var total = TimeSpan.Zero;
        for (var i = 0; i < ordenado.Count - 1; i++)
        {
            if (ordenado[i].Para == StatusOrdemServico.EmExecucao)
                total += ordenado[i + 1].Em - ordenado[i].Em;
        }
        return total;
    }

    private void MudarStatus(StatusOrdemServico novoStatus)
    {
        if (!PermitirStatus(novoStatus))
            throw new TransicaoInvalidaException(Status, novoStatus);

        var anterior = Status;
        Status = novoStatus;
        _historico.Add(new RegistroStatus(Id, anterior, novoStatus, DateTime.UtcNow));
    }

    private bool PermitirStatus(StatusOrdemServico novoStatus) => (Status, novoStatus) switch
    {
        (StatusOrdemServico.Recebida, StatusOrdemServico.EmDiagnostico) => true,
        (StatusOrdemServico.EmDiagnostico, StatusOrdemServico.AguardandoAprovacao) => true,
        (StatusOrdemServico.AguardandoAprovacao, StatusOrdemServico.EmExecucao) => true,
        (StatusOrdemServico.AguardandoAprovacao, StatusOrdemServico.Cancelada) => true,
        (StatusOrdemServico.EmExecucao, StatusOrdemServico.Finalizada) => true,
        (StatusOrdemServico.EmExecucao, StatusOrdemServico.AguardandoAprovacao) => true,
        (StatusOrdemServico.Finalizada, StatusOrdemServico.Entregue) => true,
        _ => false
    };
}