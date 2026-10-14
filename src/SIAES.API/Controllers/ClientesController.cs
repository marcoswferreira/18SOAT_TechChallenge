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
public class ClientesController(
    CreateClienteUseCase createClienteUseCase,
    GetClienteByIdUseCase getClienteByIdUseCase,
    GetClientesPagedUseCase getClientesPagedUseCase,
    UpdateClienteUseCase updateClienteUseCase,
    DeleteClienteUseCase deleteClienteUseCase) : ControllerBase
{
    private readonly CreateClienteUseCase _createClienteUseCase = createClienteUseCase;
    private readonly GetClienteByIdUseCase _getClienteByIdUseCase = getClienteByIdUseCase;
    private readonly GetClientesPagedUseCase _getClientesPagedUseCase = getClientesPagedUseCase;
    private readonly UpdateClienteUseCase _updateClienteUseCase = updateClienteUseCase;
    private readonly DeleteClienteUseCase _deleteClienteUseCase = deleteClienteUseCase;

    /// <summary>
    /// Cadastra um novo cliente no sistema.
    /// </summary>
    /// <param name="request">Os dados do cliente a ser cadastrado</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Os dados do cliente cadastrado</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ClienteOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClienteInput request,
        CancellationToken cancellationToken)
    {
        var result = await _createClienteUseCase.ExecuteAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Lista clientes paginados.
    /// </summary>
    /// <param name="pageIndex">O índice da página</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Uma lista de clientes paginados</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IPaginate<ClienteOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IPaginate<ClienteOutput> result = await _getClientesPagedUseCase.ExecuteAsync(pageIndex, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém os detalhes de um cliente específico pelo seu ID.
    /// </summary>
    /// <param name="id">O ID do cliente</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Os dados do cliente</returns>
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

    /// <summary>
    /// Atualiza os dados de um cliente existente.
    /// </summary>
    /// <param name="id">O ID do cliente</param>
    /// <param name="request">Os dados atualizados do cliente</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Os dados atualizados do cliente</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ClienteOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateClienteInput request,
        CancellationToken cancellationToken)
    {
        var result = await _updateClienteUseCase.ExecuteAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    /// <summary>
    /// Remove logicamente (Soft Delete) um cliente do sistema (Apenas Admin).
    /// </summary>
    /// <param name="id">O ID do cliente</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _deleteClienteUseCase.ExecuteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return NoContent();
    }
}