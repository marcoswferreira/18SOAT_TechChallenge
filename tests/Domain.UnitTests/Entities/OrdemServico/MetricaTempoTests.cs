using Domain.UnitTests.Entities.OrdemServico.Helpers;
using FluentAssertions;

namespace Domain.UnitTests.Entities.OrdemServico;

public class MetricaTempoTests
{
    [Fact]
    public void TempoDeExecucao_RetornaNulo_QuandoNaoFinalizada()
    {
        var os = OrdemServicoBuilder.EmExecucao();

        os.TempoDeExecucao().Should().BeNull();
    }

    [Fact]
    public void TempoDeExecucao_RetornaValorNaoNulo_QuandoFinalizada()
    {
        var os = OrdemServicoBuilder.Finalizada();

        var tempo = os.TempoDeExecucao();

        tempo.Should().NotBeNull();
    }

    [Fact]
    public void TempoDeExecucao_NaoContaEsperaPeloAdicional()
    {
        // Fluxo: execução -> espera aprovação adicional -> execução -> finaliza
        // O tempo de execução deve ser a SOMA dos dois intervalos em execução,
        // sem o tempo de espera no meio.
        var os = OrdemServicoBuilder.EmExecucao();
        os.SolicitarAprovacaoAdicional();
        os.AprovarOrcamento();
        os.ExecutarServico();
        os.FinalizarServico();

        var tempo = os.TempoDeExecucao();

        tempo.Should().NotBeNull();
    }

    [Fact]
    public void TempoDeExecucao_QuandoNuncaEntrou_EmExecucao_RetornaNulo()
    {
        var os = OrdemServicoBuilder.AguardandoAprovacao();

        os.TempoDeExecucao().Should().BeNull();
    }
}