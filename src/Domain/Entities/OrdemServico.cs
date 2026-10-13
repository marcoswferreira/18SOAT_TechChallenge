using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class OrdemServico
{
    private readonly List<ItemServico> _servicos = new();
    private readonly List<ItemPeca> _pecas = new();

    public Guid Id { get; private set; }
    public StatusOrdemServico Status { get; private set; }
    public decimal ValorTotal {  get; private set; }

    public IReadOnlyCollection<ItemServico> Servicos => _servicos.AsReadOnly();
    public IReadOnlyCollection<ItemPeca> Pecas => _pecas.AsReadOnly();

    public OrdemServico()
    {
        Id = Guid.NewGuid();
        Status = StatusOrdemServico.Recebida;
        ValorTotal = 0m;
    }

    public void AdicionarServico(ItemServico servico)
    {
        _servicos.Add(servico);
        RecalcularOrcamento();
    }

    public void AdicionarPeca(ItemPeca peca)
    {
        _pecas.Add(peca);
        RecalcularOrcamento();
    }

    private void RecalcularOrcamento()
    {
        ValorTotal = _servicos.Sum(s => s.ValorTotal) + _pecas.Sum(p => p.ValorTotal);
    }

    public void IniciarDiagnostico()
    {
        MudarStatus(StatusOrdemServico.EmDiagnostico);
    }

    public void GerarEEnviarOrcamento()
    {
        if (ValorTotal <= 0)
            throw new InvalidOperationException("Não é possível solicitar aprovação de um orçamento sem itens/valor zerado.");

        MudarStatus(StatusOrdemServico.AguardandoAprovacao);
    }

    public void AprovarOrcamento()
    {
        MudarStatus(StatusOrdemServico.EmExecucao);
    }

    public void RejeitarOrcamento()
    {
        MudarStatus(StatusOrdemServico.Cancelada);
    }

    public void SolicitarAprovaçãoAdicional()
    {
        // Fluxo de re-diagnóstico/adicionais durante a execução
        MudarStatus(StatusOrdemServico.AguardandoAprovacao);
    }

    public void FinalizarServico()
    {
        MudarStatus(StatusOrdemServico.Finalizada);
    }

    public void RegistrarEntrega()
    {
        MudarStatus(StatusOrdemServico.Entregue);
    }

    private void MudarStatus(StatusOrdemServico novoStatus)
    {
        if (!PermitirStatus(novoStatus))
        {
            throw new InvalidOperationException($"Transição inválida de status: de {Status} para {novoStatus}.");
        }

        Status = novoStatus;
    }

    private bool PermitirStatus(StatusOrdemServico novoStatus) => (Status, novoStatus) switch
    {
        (StatusOrdemServico.Recebida, StatusOrdemServico.EmDiagnostico) => true,
        (StatusOrdemServico.EmDiagnostico, StatusOrdemServico.AguardandoAprovacao) => true,
        (StatusOrdemServico.AguardandoAprovacao, StatusOrdemServico.EmExecucao) => true,
        (StatusOrdemServico.AguardandoAprovacao, StatusOrdemServico.Cancelada) => true,
        (StatusOrdemServico.EmExecucao, StatusOrdemServico.Finalizada) => true,
        (StatusOrdemServico.EmExecucao, StatusOrdemServico.AguardandoAprovacao) => true, // Serviços adicionais encontrados
        (StatusOrdemServico.Finalizada, StatusOrdemServico.Entregue) => true,
        _ => false
    };

}
