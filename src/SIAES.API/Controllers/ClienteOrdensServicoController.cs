using Application.Dto;
using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cliente/ordens-servico")]
public class ClienteOrdensServicoController(OrdemServicoService service) : ControllerBase
{
    /// <summary>Consulta o status de uma OS pelo ID.</summary>
    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(ConsultaStatusClienteResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ObterStatusPorId(Guid id, CancellationToken ct)
    {
        var resultado = await service.ConsultarStatusPorIdAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Consulta todas as OS de um cliente pelo CPF ou CNPJ.</summary>
    [HttpGet("documento/{documento}/status")]
    [ProducesResponseType(typeof(IEnumerable<ConsultaStatusClienteResponse>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ObterStatusPorDocumento(
        string documento, CancellationToken ct)
    {
        var resultado = await service.ConsultarStatusPorDocumentoAsync(documento, ct);
        return Ok(resultado);
    }
}