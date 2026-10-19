using Application.UseCases.OrdensServico;
using Application.UseCases.OrdensServico.Dto;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ordens-servico")]
public class OrdensServicoController(GetStatusOrdemServicoUseCase useCase) : ControllerBase
{
    private readonly GetStatusOrdemServicoUseCase _useCase = useCase;

    /// <summary>Consulta o status de uma OS pelo ID.</summary>
    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(GetStatusClienteResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ObterStatusPorId(Guid id, CancellationToken ct)
    {
        var resultado = await _useCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Consulta todas as OS de um cliente pelo CPF ou CNPJ.</summary>
    [HttpGet("documento/{documento}/status")]
    [ProducesResponseType(typeof(IEnumerable<GetStatusClienteResponse>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ObterStatusPorDocumento(
        string documento, CancellationToken ct)
    {
        var resultado = await _useCase.ExecuteAsync(documento, ct);
        return Ok(resultado);
    }
}