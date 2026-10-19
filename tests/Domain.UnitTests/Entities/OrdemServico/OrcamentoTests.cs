using Domain.Common;
using Domain.UnitTests.Entities.OrdemServico.Helpers;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.UnitTests.Entities.OrdemServico;

public class OrcamentoTests
{
    [Fact]
    public void AdicionarServico_RecalculaValorTotal()
    {
        var os = OrdemServicoBuilder.Nova();

        os.AdicionarServico(new ItemServico("Troca de óleo", 120.00m, 1));

        os.ValorTotal.Should().Be(120.00m);
    }

    [Fact]
    public void AdicionarPeca_RecalculaValorTotal()
    {
        var os = OrdemServicoBuilder.Nova();

        os.AdicionarPeca(new ItemPeca("Filtro", 35.50m, 2));

        os.ValorTotal.Should().Be(71.00m);
    }

    [Fact]
    public void AdicionarServicosEPecas_SomaValoresCorretamente()
    {
        var os = OrdemServicoBuilder.ComOrcamento(); // 1x120 + 2x35,50

        os.ValorTotal.Should().Be(191.00m);
    }

    [Fact]
    public void GerarOrcamento_SemItens_LancaDomainException()
    {
        var os = OrdemServicoBuilder.Nova();
        os.IniciarDiagnostico();

        var acao = () => os.GerarEEnviarOrcamento();

        acao.Should().Throw<DomainException>()
            .WithMessage("*orçamento*");
    }

    [Fact]
    public void AdicionarServico_ComItemNulo_LancaArgumentNullException()
    {
        var os = OrdemServicoBuilder.Nova();

        var acao = () => os.AdicionarServico(null!);

        acao.Should().Throw<ArgumentNullException>();
    }
}