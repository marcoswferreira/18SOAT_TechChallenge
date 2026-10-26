using Domain.Enums;
using Domain.ValueObjects;
using DomainEntities = Domain.Entities;

namespace Domain.UnitTests.Entities.OrdemServico.Helpers;

/// <summary>
/// Constrói OS em estados específicos para os testes.
/// </summary>
internal static class OrdemServicoBuilder
{
    public static readonly Guid ClienteId = Guid.NewGuid();
    public static readonly Guid VeiculoId = Guid.NewGuid();
    public static readonly Guid ServicoId = Guid.NewGuid();
    public static readonly Guid PecaId = Guid.NewGuid();

    /// OS recém-criada, sem itens
    public static DomainEntities.OrdemServico Nova()
        => new(ClienteId, VeiculoId);

    public static DomainEntities.OrdemServico ComOrcamento()
    {
        var os = Nova();
        os.AdicionarServico(new ItemServico("Troca de óleo", 120.00m, 1));
        os.AdicionarPeca(new ItemPeca("Filtro de óleo", 35.50m, 2));
        return os;
    }

    /// OS em AguardandoAprovacao (orçamento gerado)
    public static DomainEntities.OrdemServico AguardandoAprovacao()
    {
        var os = ComOrcamento();
        os.IniciarDiagnostico();
        os.GerarEEnviarOrcamento();
        return os;
    }

    /// OS em EmExecucao (orçamento aprovado e serviço iniciado)
    public static DomainEntities.OrdemServico EmExecucao()
    {
        var os = AguardandoAprovacao();
        os.AprovarOrcamento();
        os.ExecutarServico();
        return os;
    }

    /// OS Finalizada
    public static DomainEntities.OrdemServico Finalizada()
    {
        var os = EmExecucao();
        os.FinalizarServico();
        return os;
    }
}