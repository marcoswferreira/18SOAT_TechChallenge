using Application.UseCases.OrdensServico.Dto;
using Domain.Interfaces.Repositories;

namespace Application.UseCases.OrdensServico
{
    public class TempoMedioExecucaoUseCase(IOrdemServicoRepository repository)
    {
        public async Task<MetricaTempoExecucaoResponse> ExecuteAsync(
            TempoMedioExecucaoInput input, CancellationToken ct = default)
        {
            var tempos = (await repository
                .ObterTemposDeExecucaoAsync(input.De, input.Ate, ct))
                .ToList();

            if (tempos.Count == 0)
                return new MetricaTempoExecucaoResponse(0, TimeSpan.Zero);

            var media = TimeSpan.FromTicks(tempos.Sum(t => t.Ticks) / tempos.Count);
            return new MetricaTempoExecucaoResponse(tempos.Count, media);
        }
    }
}
