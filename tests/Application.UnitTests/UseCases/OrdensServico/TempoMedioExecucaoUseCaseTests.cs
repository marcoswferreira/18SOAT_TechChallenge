using Application.UseCases.OrdensServico;
using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace Application.UnitTests.UseCases.OrdensServico;

public class TempoMedioExecucaoUseCaseTests
{
    private readonly IOrdemServicoRepository _repository = Substitute.For<IOrdemServicoRepository>();
    private readonly TempoMedioExecucaoUseCase _useCase;

    public TempoMedioExecucaoUseCaseTests()
    {
        _useCase = new TempoMedioExecucaoUseCase(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_QuandoNaoHaOrdensFinalizadas_RetornaZero()
    {
        // Arrange
        var input = new TempoMedioExecucaoInput();
        _repository.ObterTemposDeExecucaoAsync(input.De, input.Ate, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var response = await _useCase.ExecuteAsync(input);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(0, response.QuantidadeOs);
        Assert.Equal(TimeSpan.Zero, response.TempoMedio);
        Assert.Equal(0.0, response.TempoMedioMinutos);
        Assert.Equal("0h 00min", response.TempoMedioFormatado);
    }

    [Fact]
    public async Task ExecuteAsync_ComMultiplasOrdens_CalculaMediaCorretamente()
    {
        // Arrange
        var de = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var ate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);
        var input = new TempoMedioExecucaoInput(de, ate);

        var tempos = new List<TimeSpan>
        {
            TimeSpan.FromHours(1), // 60 min
            TimeSpan.FromHours(3)  // 180 min -> Média = 2 horas (120 min)
        };

        _repository.ObterTemposDeExecucaoAsync(de, ate, Arg.Any<CancellationToken>())
            .Returns(tempos);

        // Act
        var response = await _useCase.ExecuteAsync(input);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.QuantidadeOs);
        Assert.Equal(TimeSpan.FromHours(2), response.TempoMedio);
        Assert.Equal(120.0, response.TempoMedioMinutos);
        Assert.Equal("2h 00min", response.TempoMedioFormatado);
    }

    [Fact]
    public void MetricaTempoExecucaoResponse_CalculaPropriedadesFormatadas()
    {
        // 25 horas e 15 minutos e 30 segundos
        var tempo = new TimeSpan(1, 1, 15, 30);
        var response = new MetricaTempoExecucaoResponse(10, tempo);

        Assert.Equal(10, response.QuantidadeOs);
        Assert.Equal(tempo, response.TempoMedio);
        Assert.Equal(Math.Round(tempo.TotalMinutes, 1), response.TempoMedioMinutos);
        Assert.Equal("25h 15min", response.TempoMedioFormatado);
    }

    [Fact]
    public void TempoMedioExecucaoInput_PermiteValoresNulosOuDefinidos()
    {
        var inputDefault = new TempoMedioExecucaoInput();
        Assert.Null(inputDefault.De);
        Assert.Null(inputDefault.Ate);

        var agora = DateTime.UtcNow;
        var inputPreenchido = new TempoMedioExecucaoInput(agora.AddDays(-7), agora);
        Assert.Equal(agora.AddDays(-7), inputPreenchido.De);
        Assert.Equal(agora, inputPreenchido.Ate);
    }
}

