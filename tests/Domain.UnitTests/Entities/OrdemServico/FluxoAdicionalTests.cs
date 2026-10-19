using Domain.Common;
using Domain.Enums;
using Domain.UnitTests.Entities.OrdemServico.Helpers;
using FluentAssertions;

namespace Domain.UnitTests.Entities.OrdemServico;

public class FluxoAdicionalTests
{
    [Fact]
    public void SolicitarAprovacaoAdicional_VoltaParaAguardandoAprovacao()
    {
        var os = OrdemServicoBuilder.EmExecucao();

        os.SolicitarAprovacaoAdicional();

        os.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao);
    }

    [Fact]
    public void AprovarOrcamentoAdicional_RetornaParaEmExecucao()
    {
        var os = OrdemServicoBuilder.EmExecucao();
        os.SolicitarAprovacaoAdicional();

        os.AprovarOrcamentoAdicional();

        os.Status.Should().Be(StatusOrdemServico.EmExecucao);
    }

    [Fact]
    public void RejeitarOrcamentoAdicional_MantemAguardandoAprovacao()
    {
        var os = OrdemServicoBuilder.EmExecucao();
        os.SolicitarAprovacaoAdicional();

        os.RejeitarOrcamentoAdicional();

        os.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao);
    }

    [Fact]
    public void AposRejeitarAdicional_PodeRetomar_ComExecutarServico()
    {
        var os = OrdemServicoBuilder.EmExecucao();
        os.SolicitarAprovacaoAdicional();
        os.RejeitarOrcamentoAdicional();

        // Atendente decide retomar com o que já foi aprovado
        os.ExecutarServico();

        os.Status.Should().Be(StatusOrdemServico.EmExecucao);
    }

    [Fact]
    public void AposRejeitarAdicional_PodeCancelar_ComRejeitarOs()
    {
        var os = OrdemServicoBuilder.EmExecucao();
        os.SolicitarAprovacaoAdicional();
        os.RejeitarOrcamentoAdicional();

        // Atendente decide cancelar
        os.RejeitarOs();

        os.Status.Should().Be(StatusOrdemServico.Cancelada);
    }

    [Fact]
    public void SolicitarAprovacaoAdicional_ForaDeEmExecucao_LancaDomainException()
    {
        var os = OrdemServicoBuilder.AguardandoAprovacao();

        var acao = () => os.SolicitarAprovacaoAdicional();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void FluxoAdicional_HistoricoRegistraTodasAsVoltas()
    {
        var os = OrdemServicoBuilder.EmExecucao();
        os.SolicitarAprovacaoAdicional();
        os.AprovarOrcamentoAdicional();
        os.FinalizarServico();

        // Recebida + EmDiagnostico + AguardandoAprovacao + EmExecucao
        // + AguardandoAprovacao + EmExecucao + Finalizada = 7
        os.Historico.Should().HaveCount(7);
    }
}