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
public class VeiculoController(
    CreateVeiculoUseCase createVeiculoUseCase,
    UpdateVeiculoUseCase updateVeiculoUseCase,
    DeleteVeiculoUseCase deleteVeiculoUseCase,
    GetVeiculoByIdUseCase getVeiculoByIdUseCase) : ControllerBase
{
    private readonly CreateVeiculoUseCase _createVeiculoUseCase = createVeiculoUseCase;

    private readonly UpdateVeiculoUseCase _updateVeiculoUseCase = updateVeiculoUseCase;
    
    private readonly DeleteVeiculoUseCase _deleteVeiculoUseCase = deleteVeiculoUseCase;

    private readonly GetVeiculoByIdUseCase _getVeiculoByIdUseCase = getVeiculoByIdUseCase;

    public async Task<IActionResult> Create(
        [FromBody] CreateVeiculoInput request,
        CancellationToken cancellationToken)
    {
        var result = await _createVeiculoUseCase.ExecuteAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(VeiculoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateVeiculoInput request,
        CancellationToken cancellationToken)
    {
        VeiculoOutput result = await _updateVeiculoUseCase.ExecuteAsync(id, request, cancellationToken);
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
        await _deleteVeiculoUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

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
}