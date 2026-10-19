using Domain.Common;
using Domain.Enums;
using Domain.UnitTests.Entities.OrdemServico.Helpers;
using FluentAssertions;

namespace Domain.UnitTests.Entities.OrdemServico;

public class FluxoPrincipalTests
{
    [Fact]
    public void FluxoCompleto_PercorreTodosOsStatusEmOrdem()
    {
        var os = OrdemServicoBuilder.ComOrcamento();

        os.IniciarDiagnostico();
        os.Status.Should().Be(StatusOrdemServico.EmDiagnostico);

        os.GerarEEnviarOrcamento();
        os.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao);

        os.AprovarOrcamento();
        os.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao);

        os.ExecutarServico();
        os.Status.Should().Be(StatusOrdemServico.EmExecucao);

        os.FinalizarServico();
        os.Status.Should().Be(StatusOrdemServico.Finalizada);

        os.RegistrarEntrega();
        os.Status.Should().Be(StatusOrdemServico.Entregue);
    }

    [Fact]
    public void FluxoCompleto_HistoricoRegistraTodasAsTransicoes()
    {
        var os = OrdemServicoBuilder.Finalizada();
        os.RegistrarEntrega();

        // Criação + 5 transições = 6 registros
        os.Historico.Should().HaveCount(6);

        var statuses = os.Historico.Select(r => r.Para).ToList();
        statuses.Should().ContainInOrder(
            StatusOrdemServico.Recebida,
            StatusOrdemServico.EmDiagnostico,
            StatusOrdemServico.AguardandoAprovacao,
            StatusOrdemServico.EmExecucao,
            StatusOrdemServico.Finalizada,
            StatusOrdemServico.Entregue);
    }

    [Fact]
    public void AprovarOrcamento_ForaDeAguardandoAprovacao_LancaDomainException()
    {
        var os = OrdemServicoBuilder.Nova(); // Recebida

        var acao = () => os.AprovarOrcamento();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void ExecutarServico_ForaDeAguardandoAprovacao_LancaTransicaoInvalidaException()
    {
        var os = OrdemServicoBuilder.Nova(); // Recebida

        var acao = () => os.ExecutarServico();

        acao.Should().Throw<DomainException>();
    }
}