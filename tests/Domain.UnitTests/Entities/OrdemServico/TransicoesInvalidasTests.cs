using Domain.Common;
using Domain.Enums;
using Domain.UnitTests.Entities.OrdemServico.Helpers;
using FluentAssertions;

namespace Domain.UnitTests.Entities.OrdemServico;

public class TransicoesInvalidasTests
{
    [Fact]
    public void NaoPermite_PularDiagnostico()
    {
        var os = OrdemServicoBuilder.ComOrcamento(); // Recebida

        var acao = () => os.GerarEEnviarOrcamento();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void NaoPermite_FinalizarSemExecutar()
    {
        var os = OrdemServicoBuilder.ComOrcamento(); // Recebida

        var acao = () => os.FinalizarServico();

        acao.Should().Throw<TransicaoInvalidaException>();
    }

    [Fact]
    public void NaoPermite_EntregarSemFinalizar()
    {
        var os = OrdemServicoBuilder.EmExecucao();

        var acao = () => os.RegistrarEntrega();

        acao.Should().Throw<TransicaoInvalidaException>();
    }

    [Fact]
    public void NaoPermite_NenhumaTransicao_ApartirDeEntregue()
    {
        var os = OrdemServicoBuilder.Finalizada();
        os.RegistrarEntrega();

        // Status terminal: nenhuma ação é permitida
        var acoes = new List<Action>
        {
            () => os.IniciarDiagnostico(),
            () => os.GerarEEnviarOrcamento(),
            () => os.ExecutarServico(),
            () => os.FinalizarServico(),
            () => os.RegistrarEntrega()
        };

        acoes.Should().AllSatisfy(acao =>
            acao.Should().Throw<DomainException>());
    }

    [Fact]
    public void NaoPermite_NenhumaTransicao_ApartirDeCancelada()
    {
        var os = OrdemServicoBuilder.AguardandoAprovacao();
        os.RejeitarOrcamento();

        os.Status.Should().Be(StatusOrdemServico.Cancelada);

        var acao = () => os.IniciarDiagnostico();

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransicaoInvalida_MensagemContemStatusAtualEDestino()
    {
        var os = OrdemServicoBuilder.Nova(); // Recebida

        var acao = () => os.FinalizarServico();

        acao.Should().Throw<TransicaoInvalidaException>()
            .WithMessage("*Recebida*Finalizada*");
    }
}