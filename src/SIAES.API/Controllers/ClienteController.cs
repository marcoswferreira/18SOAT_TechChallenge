using Application.UseCases.Clientes;
using Application.UseCases.Clientes.Dto;
using Asp.Versioning;
using Domain.Common.Paginate;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ClienteController(
    CreateClienteUseCase createClienteUseCase,
    GetClienteByIdUseCase getClienteByIdUseCase) : ControllerBase
{
    private readonly CreateClienteUseCase _createClienteUseCase = createClienteUseCase;

    private readonly GetClienteByIdUseCase _getClienteByIdUseCase = getClienteByIdUseCase;

    public async Task<IActionResult> Create(
        [FromBody] CreateClienteInput request,
        CancellationToken cancellationToken)
    {
        var result = await _createClienteUseCase.ExecuteAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ClienteOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        ClienteOutput result = await _getClienteByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }
}