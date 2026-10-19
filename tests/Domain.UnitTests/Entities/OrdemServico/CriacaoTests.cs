using Domain.Common;
using Domain.Enums;
using Domain.UnitTests.Entities.OrdemServico.Helpers;
using FluentAssertions;
using DomainEntities = Domain.Entities;

namespace Domain.UnitTests.Entities.OrdemServico;

public class CriacaoTests
{
    [Fact]
    public void OS_SempreNasceComStatusRecebida()
    {
        var os = OrdemServicoBuilder.Nova();

        os.Status.Should().Be(StatusOrdemServico.Recebida);
    }

    [Fact]
    public void OS_RegistraHistoricoNaCriacao()
    {
        var os = OrdemServicoBuilder.Nova();

        os.Historico.Should().HaveCount(1);
        os.Historico.First().De.Should().BeNull();
        os.Historico.First().Para.Should().Be(StatusOrdemServico.Recebida);
    }

    [Fact]
    public void OS_ComClienteIdVazio_LancaDomainException()
    {
        var acao = () => new DomainEntities.OrdemServico(Guid.Empty, Guid.NewGuid());

        acao.Should().Throw<DomainException>()
            .WithMessage("*Cliente*");
    }

    [Fact]
    public void OS_ComVeiculoIdVazio_LancaDomainException()
    {
        var acao = () => new DomainEntities.OrdemServico(Guid.NewGuid(), Guid.Empty);

        acao.Should().Throw<DomainException>()
            .WithMessage("*Veículo*");
    }

    [Fact]
    public void OS_ValorTotalComecaZerado()
    {
        var os = OrdemServicoBuilder.Nova();

        os.ValorTotal.Should().Be(0m);
    }
}