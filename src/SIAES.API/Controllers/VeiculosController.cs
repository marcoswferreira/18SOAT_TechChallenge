using Application.UseCases.Veiculos;
using Application.UseCases.Veiculos.Dto;
using Asp.Versioning;
using Domain.Common.Paginate;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class VeiculosController(
    CreateVeiculoUseCase createVeiculoUseCase,
    GetVeiculoByIdUseCase getVeiculoByIdUseCase,
    GetVeiculosPagedUseCase getVeiculosPagedUseCase,
    UpdateVeiculoUseCase updateVeiculoUseCase,
    DeleteVeiculoUseCase deleteVeiculoUseCase) : ControllerBase
{
    private readonly CreateVeiculoUseCase _createVeiculoUseCase = createVeiculoUseCase;
    private readonly GetVeiculoByIdUseCase _getVeiculoByIdUseCase = getVeiculoByIdUseCase;
    private readonly GetVeiculosPagedUseCase _getVeiculosPagedUseCase = getVeiculosPagedUseCase;
    private readonly UpdateVeiculoUseCase _updateVeiculoUseCase = updateVeiculoUseCase;
    private readonly DeleteVeiculoUseCase _deleteVeiculoUseCase = deleteVeiculoUseCase;

    /// <summary>
    /// Cadastra um novo veículo no sistema.
    /// </summary>
    /// <param name="request">Os dados do veículo a ser cadastrado</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Os dados do veículo cadastrado</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(VeiculoOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVeiculoInput request,
        CancellationToken cancellationToken)
    {
        var result = await _createVeiculoUseCase.ExecuteAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Lista veículos paginados.
    /// </summary>
    /// <param name="pageIndex">O índice da página</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Uma lista de veículos paginados</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IPaginate<VeiculoOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IPaginate<VeiculoOutput> result = await _getVeiculosPagedUseCase.ExecuteAsync(pageIndex, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém os detalhes de um veículo específico pelo seu ID.
    /// </summary>
    /// <param name="id">O ID do veículo</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Os dados do veículo</returns>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(VeiculoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        VeiculoOutput result = await _getVeiculoByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza os dados de um veículo existente.
    /// </summary>
    /// <param name="id">O ID do veículo</param>
    /// <param name="request">Os dados atualizados do veículo</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Os dados atualizados do veículo</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(VeiculoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateVeiculoInput request,
        CancellationToken cancellationToken)
    {
        var result = await _updateVeiculoUseCase.ExecuteAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    /// <summary>
    /// Remove logicamente (Soft Delete) um veículo do sistema (Apenas Admin).
    /// </summary>
    /// <param name="id">O ID do veículo</param>
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
        var result = await _deleteVeiculoUseCase.ExecuteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return NoContent();
    }
}