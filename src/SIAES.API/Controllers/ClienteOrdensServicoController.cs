using Application.Dto;
using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cliente/ordens-servico")]
public class ClienteOrdensServicoController(OrdemServicoService service) : ControllerBase
{
    /// <summary>
    /// Consulta o status de uma Ordem de Serviço pelo ID/Código enviado ao cliente.
    /// </summary>
    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(ConsultaStatusClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterStatusPorId(Guid id, CancellationToken ct)
    {
        try
        {
            var status = await service.ConsultarStatusPorIdAsync(id, ct);
            return Ok(status);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Consulta todas as Ordens de Serviço vinculadas ao CPF ou CNPJ do cliente.
    /// </summary>
    [HttpGet("documento/{documento}/status")]
    [ProducesResponseType(typeof(IEnumerable<ConsultaStatusClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterStatusPorDocumento(string documento, CancellationToken ct)
    {
        try
        {
            var ordens = await service.ConsultarStatusPorDocumentoAsync(documento, ct);
            return Ok(ordens);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}
