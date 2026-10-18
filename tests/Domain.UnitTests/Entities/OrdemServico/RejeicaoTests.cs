using Domain.Common;
using Domain.Enums;
using Domain.UnitTests.Entities.OrdemServico.Helpers;
using FluentAssertions;

namespace Domain.UnitTests.Entities.OrdemServico;

public class RejeicaoTests
{
    [Fact]
    public void RejeitarOrcamento_CancelaAOs()
    {
        var os = OrdemServicoBuilder.AguardandoAprovacao();

        os.RejeitarOrcamento();

        os.Status.Should().Be(StatusOrdemServico.Cancelada);
    }

    [Fact]
    public void RejeitarOrcamento_ForaDeAguardandoAprovacao_LancaDomainException()
    {
        var os = OrdemServicoBuilder.Nova(); // Recebida

        var acao = () => os.RejeitarOrcamento();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void RejeitarOs_ForaDeAguardandoAprovacao_LancaDomainException()
    {
        var os = OrdemServicoBuilder.EmExecucao();

        var acao = () => os.RejeitarOs();

        acao.Should().Throw<DomainException>();
    }
}