using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database.DbContexts;
using Infrastructure.Database.Repositories;
using IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace IntegrationTests.Repositories;

public class OrdemServicoRepositoryTests
{
    private static OrdemServico CriarOrdemFinalizada()
    {
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        os.IniciarDiagnostico();
        os.AdicionarServico(new Servico("Troca de Óleo"), 1, 100);
        os.GerarEEnviarOrcamento();
        os.ExecutarServico();
        os.FinalizarServico();
        return os;
    }

    [Fact]
    public async Task ObterTemposDeExecucaoAsync_ComOrdemFinalizada_RetornaTemposValidos()
    {
        // Arrange
        var userCtx = Substitute.For<IUserContext>();
        var dbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptions<ApplicationDbContext>(), userCtx);

        var os = CriarOrdemFinalizada();
        var ordens = new List<OrdemServico> { os };
        var mockDbSet = DbSetMockExtensions.CreateMockDbSet(ordens);

        dbContext.Set<OrdemServico>().Returns(mockDbSet);
        dbContext.OrdensServico.Returns(mockDbSet);

        var repository = new OrdemServicoRepository(dbContext);

        // Act
        var de = DateTime.UtcNow.AddDays(-1);
        var ate = DateTime.UtcNow.AddDays(1);
        var tempos = (await repository.ObterTemposDeExecucaoAsync(de, ate)).ToList();

        // Assert
        Assert.NotNull(tempos);
        Assert.Single(tempos);
    }

    [Fact]
    public async Task ObterTemposDeExecucaoAsync_SemFiltros_ExecutaSemErros()
    {
        // Arrange
        var userCtx = Substitute.For<IUserContext>();
        var dbContext = Substitute.For<ApplicationDbContext>(
            new DbContextOptions<ApplicationDbContext>(), userCtx);

        var ordens = new List<OrdemServico>();
        var mockDbSet = DbSetMockExtensions.CreateMockDbSet(ordens);

        dbContext.Set<OrdemServico>().Returns(mockDbSet);
        dbContext.OrdensServico.Returns(mockDbSet);

        var repository = new OrdemServicoRepository(dbContext);

        // Act
        var tempos = (await repository.ObterTemposDeExecucaoAsync(null, null)).ToList();

        // Assert
        Assert.NotNull(tempos);
        Assert.Empty(tempos);
    }
}

