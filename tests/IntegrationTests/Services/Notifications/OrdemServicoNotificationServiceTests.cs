using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.ValueObjects;
using Infrastructure.Services.Notifications;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace IntegrationTests.Services.Notifications;

public class OrdemServicoNotificationServiceTests
{
    private readonly IResendEmailService _emailService = Substitute.For<IResendEmailService>();
    private readonly IClienteRepository _clienteRepo = Substitute.For<IClienteRepository>();
    private readonly IVeiculoRepository _veiculoRepo = Substitute.For<IVeiculoRepository>();
    private readonly ILogger<OrdemServicoNotificationService> _logger = Substitute.For<ILogger<OrdemServicoNotificationService>>();
    private readonly OrdemServicoNotificationService _service;

    public OrdemServicoNotificationServiceTests()
    {
        _service = new OrdemServicoNotificationService(_emailService, _clienteRepo, _veiculoRepo, _logger);
    }

    private static Endereco ValidEndereco() => new("Rua B", "123", null, "Centro", "São Paulo", "SP", "01001-000");

    [Fact]
    public async Task NotificarOrdemServicoCriadaAsync_QuandoClienteTemEmail_DeveEnviarEmail()
    {
        // Arrange
        var cliente = new Cliente("Carlos Silva", new Documento("529.982.247-25"), "carlos@teste.com", "1199999999", ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("BRA2E19"), "Toyota", "Corolla", 2022);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        // Act
        await _service.NotificarOrdemServicoCriadaAsync(os, cliente, veiculo, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendAsync(
            "carlos@teste.com",
            Arg.Is<string>(s => s.Contains("Veículo Recebido")),
            Arg.Is<string>(b => b.Contains("Carlos Silva") && b.Contains("Toyota Corolla") && b.Contains("BRA2E19") && b.Contains("Recebida")),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task NotificarOrdemServicoCriadaAsync_QuandoClienteNaoTemEmail_NaoDeveChamarServicoEmail()
    {
        // Arrange
        var cliente = new Cliente("Carlos Silva", new Documento("529.982.247-25"), null, "1199999999", ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("BRA2E19"), "Toyota", "Corolla", 2022);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        // Act
        await _service.NotificarOrdemServicoCriadaAsync(os, cliente, veiculo, CancellationToken.None);

        // Assert
        await _emailService.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task NotificarOrcamentoParaAprovacaoAsync_ComServicosEPecas_DeveEnviarEmailComDetalhesEValores()
    {
        // Arrange
        var cliente = new Cliente("Ana Souza", new Documento("529.982.247-25"), "ana@teste.com", null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Honda", "Civic", 2021);
        var os = new OrdemServico(cliente.Id, veiculo.Id);
        os.IniciarDiagnostico();

        os.AdicionarServico(new ItemServico("Troca de Pastilhas de Freio", 150m, 1));
        os.AdicionarPeca(new ItemPeca(Guid.NewGuid(), "Jogo de Pastilhas", 220m, 1));
        os.GerarEEnviarOrcamento();

        // Act
        await _service.NotificarOrcamentoParaAprovacaoAsync(os, cliente, veiculo, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendAsync(
            "ana@teste.com",
            Arg.Is<string>(s => s.Contains("Orçamento Disponível para Aprovação")),
            Arg.Is<string>(b =>
                b.Contains("Ana Souza") &&
                b.Contains("Troca de Pastilhas de Freio") &&
                b.Contains("Jogo de Pastilhas") &&
                b.Contains("Aguardando Aprovação")),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task NotificarOrcamentoAprovadoAsync_DeveEnviarEmailComStatusEmExecucao()
    {
        // Arrange
        var cliente = new Cliente("Ana Souza", new Documento("529.982.247-25"), "ana@teste.com", null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Honda", "Civic", 2021);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        // Act
        await _service.NotificarOrcamentoAprovadoAsync(os, cliente, veiculo, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendAsync(
            "ana@teste.com",
            Arg.Is<string>(s => s.Contains("Orçamento Aprovado")),
            Arg.Is<string>(b => b.Contains("Em Execução")),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task NotificarOrcamentoRejeitadoAsync_DeveEnviarEmailComStatusCancelada()
    {
        // Arrange
        var cliente = new Cliente("Ana Souza", new Documento("529.982.247-25"), "ana@teste.com", null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Honda", "Civic", 2021);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        // Act
        await _service.NotificarOrcamentoRejeitadoAsync(os, cliente, veiculo, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendAsync(
            "ana@teste.com",
            Arg.Is<string>(s => s.Contains("Orçamento Rejeitado")),
            Arg.Is<string>(b => b.Contains("Cancelada")),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task NotificarServicoFinalizadoAsync_DeveEnviarEmailComStatusProntoParaRetirada()
    {
        // Arrange
        var cliente = new Cliente("Ana Souza", new Documento("529.982.247-25"), "ana@teste.com", null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Honda", "Civic", 2021);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        // Act
        await _service.NotificarServicoFinalizadoAsync(os, cliente, veiculo, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendAsync(
            "ana@teste.com",
            Arg.Is<string>(s => s.Contains("Veículo Pronto para Retirada")),
            Arg.Is<string>(b => b.Contains("Pronto para Retirada")),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task NotificarVeiculoEntregueAsync_DeveEnviarEmailComStatusEntregue()
    {
        // Arrange
        var cliente = new Cliente("Ana Souza", new Documento("529.982.247-25"), "ana@teste.com", null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Honda", "Civic", 2021);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        // Act
        await _service.NotificarVeiculoEntregueAsync(os, cliente, veiculo, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendAsync(
            "ana@teste.com",
            Arg.Is<string>(s => s.Contains("Veículo Entregue")),
            Arg.Is<string>(b => b.Contains("Entregue")),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task EnviarEmail_QuandoResendLancarExcecao_NaoDevePropagarExcecao()
    {
        // Arrange
        var cliente = new Cliente("Ana Souza", new Documento("529.982.247-25"), "ana@teste.com", null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("ABC1234"), "Honda", "Civic", 2021);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        _emailService.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Falha na comunicação com o provedor Resend."));

        // Act - Não deve lançar exceção
        var exception = await Record.ExceptionAsync(() =>
            _service.NotificarOrcamentoParaAprovacaoAsync(os, cliente, veiculo, CancellationToken.None));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task ObterClienteEVeiculo_QuandoNaoPassadosComoParametro_DeveBuscarNosRepositorios()
    {
        // Arrange
        var cliente = new Cliente("Marcos", new Documento("529.982.247-25"), "marcos@teste.com", null, ValidEndereco());
        var veiculo = new Veiculo(cliente.Id, new Placa("XYZ9876"), "Ford", "Ka", 2019);
        var os = new OrdemServico(cliente.Id, veiculo.Id);

        _clienteRepo.GetByIdAsync(cliente.Id, Arg.Any<CancellationToken>()).Returns(cliente);
        _veiculoRepo.GetByIdAsync(veiculo.Id, Arg.Any<CancellationToken>()).Returns(veiculo);

        // Act (Passando cliente e veiculo null para testar resolução via repositórios)
        await _service.NotificarOrdemServicoCriadaAsync(os, null, null, CancellationToken.None);

        // Assert
        await _clienteRepo.Received(1).GetByIdAsync(cliente.Id, Arg.Any<CancellationToken>());
        await _veiculoRepo.Received(1).GetByIdAsync(veiculo.Id, Arg.Any<CancellationToken>());
        await _emailService.Received(1).SendAsync(
            "marcos@teste.com",
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        );
    }
}

