using Domain.Interfaces.Services;
using Application.UseCases.OrdensServico;
using Application.UseCases.OrdensServico.Dto;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.UnitTests.UseCases.OrdensServico;

public class OrdemServicoLifecycleUseCasesTests
{
    private readonly IOrdemServicoRepository _osRepo = Substitute.For<IOrdemServicoRepository>();
    private readonly IItemEstoqueRepository _estoqueRepo = Substitute.For<IItemEstoqueRepository>();
    private readonly IServicoRepository _servicoRepo = Substitute.For<IServicoRepository>();
    private readonly IOrdemServicoNotificationService _notificationService = Substitute.For<IOrdemServicoNotificationService>();

    [Fact]
    public async Task IniciarDiagnostico_DeveMudarStatusParaEmDiagnostico()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);
        var useCase = new IniciarDiagnosticoUseCase(_osRepo);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.EmDiagnostico.ToString(), result.Status);
        _osRepo.Received(1).Update(os);
        await _osRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AdicionarPeca_DeveReservarEstoqueEAdicionarPecaNaOS()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        var itemEstoque = new ItemEstoque("FLT-001", "Filtro", null, UnidadeMedida.Unidade, 50m);
        itemEstoque.AdicionarEstoque(10); // Saldo disponível: 10

        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);
        _estoqueRepo.GetByIdAsync(itemEstoque.Id, Arg.Any<CancellationToken>()).Returns(itemEstoque);

        var useCase = new AdicionarPecaOrdemServicoUseCase(_osRepo, _estoqueRepo);
        var input = new AdicionarPecaInput(itemEstoque.Id, 2);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, input, CancellationToken.None);

        // Assert
        Assert.Single(result.Pecas);
        Assert.Equal(2, itemEstoque.QuantidadeReservada);
        Assert.Equal(8, itemEstoque.QuantidadeDisponivel);
        _estoqueRepo.Received(1).Update(itemEstoque);
        await _osRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AdicionarServico_ComServicoExistente_DeveAdicionarServicoNaOS()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        var servico = new Servico("Troca de Óleo");

        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);
        _servicoRepo.GetByIdAsync(servico.Id, Arg.Any<CancellationToken>()).Returns(servico);

        var useCase = new AdicionarServicoOrdemServicoUseCase(_osRepo, _servicoRepo);
        var input = new AdicionarServicoInput(servico.Id, null, 100m, 1);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, input, CancellationToken.None);

        // Assert
        Assert.Single(result.Servicos);
        Assert.Equal(100m, result.ValorTotal);
    }

    [Fact]
    public async Task GerarOrcamento_ComItens_DeveMudarParaAguardandoAprovacao_ENotificarCliente()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        os.IniciarDiagnostico();
        os.AdicionarServico(new ItemServico("Serviço", 100m, 1));
        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);

        var useCase = new GerarOrcamentoUseCase(_osRepo, _notificationService);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao.ToString(), result.Status);
        await _notificationService.Received(1).NotificarOrcamentoParaAprovacaoAsync(os, null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AprovarOrcamento_DeveMudarParaEmExecucao_ENotificarCliente()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        os.IniciarDiagnostico();
        os.AdicionarServico(new ItemServico("Serviço", 100m, 1));
        os.GerarEEnviarOrcamento();
        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);

        var useCase = new AprovarOrcamentoUseCase(_osRepo, _notificationService);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.EmExecucao.ToString(), result.Status);
        await _notificationService.Received(1).NotificarOrcamentoAprovadoAsync(os, null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RejeitarOrcamento_DeveCancelarOS_ELiberarReservaNoEstoque_ENotificarCliente()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        var itemEstoque = new ItemEstoque("FLT-001", "Filtro", null, UnidadeMedida.Unidade, 50m);
        itemEstoque.AdicionarEstoque(10);
        itemEstoque.Reservar(2);

        os.IniciarDiagnostico();
        os.AdicionarPeca(itemEstoque, 2);
        os.GerarEEnviarOrcamento();

        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);
        _estoqueRepo.GetByIdAsync(itemEstoque.Id, Arg.Any<CancellationToken>()).Returns(itemEstoque);

        var useCase = new RejeitarOrcamentoUseCase(_osRepo, _estoqueRepo, _notificationService);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.Cancelada.ToString(), result.Status);
        Assert.Equal(0, itemEstoque.QuantidadeReservada);
        Assert.Equal(10, itemEstoque.QuantidadeDisponivel);
        _estoqueRepo.Received(1).Update(itemEstoque);
        await _notificationService.Received(1).NotificarOrcamentoRejeitadoAsync(os, null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FinalizarOrdemServico_DeveMudarParaFinalizada_EDarBaixaNoEstoque_ENotificarCliente()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        var itemEstoque = new ItemEstoque("FLT-001", "Filtro", null, UnidadeMedida.Unidade, 50m);
        itemEstoque.AdicionarEstoque(10);
        itemEstoque.Reservar(2);

        os.IniciarDiagnostico();
        os.AdicionarPeca(itemEstoque, 2);
        os.GerarEEnviarOrcamento();
        os.AprovarOrcamento();
        os.ExecutarServico();

        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);
        _estoqueRepo.GetByIdAsync(itemEstoque.Id, Arg.Any<CancellationToken>()).Returns(itemEstoque);

        var useCase = new FinalizarOrdemServicoUseCase(_osRepo, _estoqueRepo, _notificationService);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.Finalizada.ToString(), result.Status);
        Assert.Equal(8, itemEstoque.QuantidadeEmEstoque);
        Assert.Equal(0, itemEstoque.QuantidadeReservada);
        Assert.Equal(8, itemEstoque.QuantidadeDisponivel);
        _estoqueRepo.Received(1).Update(itemEstoque);
        await _notificationService.Received(1).NotificarServicoFinalizadoAsync(os, null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EntregarVeiculo_DeveMudarParaEntregue_ENotificarCliente()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        os.IniciarDiagnostico();
        os.AdicionarServico(new ItemServico("Serviço", 100m, 1));
        os.GerarEEnviarOrcamento();
        os.AprovarOrcamento();
        os.ExecutarServico();
        os.FinalizarServico();

        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);

        var useCase = new EntregarVeiculoUseCase(_osRepo, _notificationService);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.Entregue.ToString(), result.Status);
        await _notificationService.Received(1).NotificarVeiculoEntregueAsync(os, null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SolicitarAprovacaoAdicional_QuandoEmExecucao_DeveMudarParaAguardandoAprovacao_ENotificarCliente()
    {
        // Arrange
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        os.IniciarDiagnostico();
        os.AdicionarServico(new ItemServico("Serviço", 100m, 1));
        os.GerarEEnviarOrcamento();
        os.AprovarOrcamento();
        os.ExecutarServico();

        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);

        var useCase = new SolicitarAprovacaoAdicionalUseCase(_osRepo, _notificationService);

        // Act
        var result = await useCase.ExecuteAsync(os.Id, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao.ToString(), result.Status);
        _osRepo.Received(1).Update(os);
        await _osRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _notificationService.Received(1).NotificarOrcamentoParaAprovacaoAsync(os, null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SolicitarAprovacaoAdicional_QuandoNaoEncontrada_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        _osRepo.GetByIdWithDetailsAsync(idInexistente, Arg.Any<CancellationToken>()).Returns((OrdemServico?)null);

        var useCase = new SolicitarAprovacaoAdicionalUseCase(_osRepo, _notificationService);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecuteAsync(idInexistente, CancellationToken.None));
    }

    [Fact]
    public async Task SolicitarAprovacaoAdicional_QuandoForaDeEmExecucao_DeveLancarDomainException()
    {
        // Arrange (OS em Diagnóstico, não em Execução)
        var os = new OrdemServico(Guid.NewGuid(), Guid.NewGuid());
        os.IniciarDiagnostico();
        _osRepo.GetByIdWithDetailsAsync(os.Id, Arg.Any<CancellationToken>()).Returns(os);

        var useCase = new SolicitarAprovacaoAdicionalUseCase(_osRepo, _notificationService);

        // Act & Assert
        await Assert.ThrowsAsync<Domain.Common.DomainException>(() =>
            useCase.ExecuteAsync(os.Id, CancellationToken.None));
    }
}

