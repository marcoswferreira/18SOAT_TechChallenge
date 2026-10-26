namespace Application.UseCases.OrdensServico.Dto;

public record MetricaTempoExecucaoResponse(
    long QuantidadeOs,
    TimeSpan TempoMedio)
{
    public double TempoMedioMinutos => Math.Round(TempoMedio.TotalMinutes, 1);
    public string TempoMedioFormatado => $"{(int)TempoMedio.TotalHours}h {TempoMedio.Minutes:00}min";
}