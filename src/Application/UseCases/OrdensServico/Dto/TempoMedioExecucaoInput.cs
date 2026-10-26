namespace Application.UseCases.OrdensServico.Dto;

public record TempoMedioExecucaoInput(
    DateTime? De = null,
    DateTime? Ate = null);
