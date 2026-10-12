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
    UpdateClienteUseCase updateClienteUseCase,
    DeleteClienteUseCase deleteClienteUseCase,
    GetClienteByIdUseCase getClienteByIdUseCase) : ControllerBase
{
    private readonly CreateClienteUseCase _createClienteUseCase = createClienteUseCase;

    private readonly UpdateClienteUseCase _updateClienteUseCase = updateClienteUseCase;
    
    private readonly DeleteClienteUseCase _deleteClienteUseCase = deleteClienteUseCase;

    private readonly GetClienteByIdUseCase _getClienteByIdUseCase = getClienteByIdUseCase;

    public async Task<IActionResult> Create(
        [FromBody] CreateClienteInput request,
        CancellationToken cancellationToken)
    {
        var result = await _createClienteUseCase.ExecuteAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ClienteOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateClienteInput request,
        CancellationToken cancellationToken)
    {
        ClienteOutput result = await _updateClienteUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await _deleteClienteUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
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