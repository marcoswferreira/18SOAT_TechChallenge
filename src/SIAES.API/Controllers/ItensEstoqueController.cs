using Application.UseCases.ItensEstoque;
using Application.UseCases.ItensEstoque.Dto;
using Asp.Versioning;
using Domain.Common.Paginate;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ItensEstoqueController(
    CreateItemEstoqueUseCase createItemEstoqueUseCase,
    GetItemEstoqueByIdUseCase getItemEstoqueByIdUseCase,
    GetItensEstoquePagedUseCase getItensEstoquePagedUseCase,
    UpdateItemEstoqueUseCase updateItemEstoqueUseCase,
    DeleteItemEstoqueUseCase deleteItemEstoqueUseCase,
    RegistrarEntradaEstoqueUseCase registrarEntradaEstoqueUseCase) : ControllerBase
{
    private const string PerfisEstoque = $"{Roles.Admin},{Roles.Manager},{Roles.Operacional}";

    private readonly CreateItemEstoqueUseCase _createItemEstoqueUseCase = createItemEstoqueUseCase;
    private readonly GetItemEstoqueByIdUseCase _getItemEstoqueByIdUseCase = getItemEstoqueByIdUseCase;
    private readonly GetItensEstoquePagedUseCase _getItensEstoquePagedUseCase = getItensEstoquePagedUseCase;
    private readonly UpdateItemEstoqueUseCase _updateItemEstoqueUseCase = updateItemEstoqueUseCase;
    private readonly DeleteItemEstoqueUseCase _deleteItemEstoqueUseCase = deleteItemEstoqueUseCase;
    private readonly RegistrarEntradaEstoqueUseCase _registrarEntradaEstoqueUseCase = registrarEntradaEstoqueUseCase;

    /// <summary>
    /// Cadastra um novo item de estoque (peça ou insumo). O estoque começa zerado.
    /// </summary>
    /// <param name="request">Os dados do item</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>O item cadastrado</returns>
    [HttpPost]
    [Authorize(Roles = PerfisEstoque)]
    [ProducesResponseType(typeof(ItemEstoqueOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateItemEstoqueInput request,
        CancellationToken cancellationToken)
    {
        var result = await _createItemEstoqueUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result.Id }, result);
    }

    /// <summary>
    /// Lista os itens de estoque de forma paginada.
    /// </summary>
    /// <param name="pageIndex">O índice da página</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Uma lista paginada de itens</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IPaginate<ItemEstoqueOutput> result = await _getItensEstoquePagedUseCase.ExecuteAsync(pageIndex, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um item pelo ID, com os saldos de estoque.
    /// </summary>
    /// <param name="id">O ID do item</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>Os dados do item</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ItemEstoqueOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        ItemEstoqueOutput result = await _getItemEstoqueByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza os dados cadastrais de um item. O saldo não é alterado por aqui.
    /// </summary>
    /// <param name="id">O ID do item</param>
    /// <param name="request">Os dados para atualização</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>O item atualizado</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = PerfisEstoque)]
    [ProducesResponseType(typeof(ItemEstoqueOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateItemEstoqueInput request,
        CancellationToken cancellationToken)
    {
        ItemEstoqueOutput result = await _updateItemEstoqueUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Remove logicamente (Soft Delete) um item. Não é permitido se houver saldo reservado.
    /// </summary>
    /// <param name="id">O ID do item</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await _deleteItemEstoqueUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Registra a entrada (reposição) de uma quantidade no estoque do item.
    /// </summary>
    /// <param name="id">O ID do item</param>
    /// <param name="request">A quantidade recebida</param>
    /// <param name="cancellationToken">O token de cancelamento</param>
    /// <returns>O item com o saldo atualizado</returns>
    [HttpPost("{id:guid}/entradas")]
    [Authorize(Roles = PerfisEstoque)]
    [ProducesResponseType(typeof(ItemEstoqueOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegistrarEntrada(
        [FromRoute] Guid id,
        [FromBody] EntradaEstoqueInput request,
        CancellationToken cancellationToken)
    {
        ItemEstoqueOutput result = await _registrarEntradaEstoqueUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(result);
    }
}
