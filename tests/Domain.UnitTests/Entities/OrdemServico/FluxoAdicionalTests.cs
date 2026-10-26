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
    public void AposRejeitarAdicional_PodeCancelar_ComRejeitarOs()
    {
        var os = OrdemServicoBuilder.EmExecucao();
        os.SolicitarAprovacaoAdicional();
        os.RejeitarOrcamento();

        os.Status.Should().Be(StatusOrdemServico.Cancelada);
    }

    [Fact]
    public void SolicitarAprovacaoAdicional_ForaDeEmExecucao_LancaDomainException()
    {
        var os = OrdemServicoBuilder.AguardandoAprovacao();

        var acao = () => os.SolicitarAprovacaoAdicional();

        acao.Should().Throw<DomainException>();
    }
}