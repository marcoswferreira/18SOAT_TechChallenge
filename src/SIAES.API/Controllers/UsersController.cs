using Application.UseCases.Users;
using Application.UseCases.Users.Dto;
using Asp.Versioning;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController(
    CreateUserUseCase createUserUseCase,
    GetUserByIdUseCase getUserByIdUseCase,
    GetUsersPagedUseCase getUsersPagedUseCase,
    UpdateUserUseCase updateUserUseCase,
    DeleteUserUseCase deleteUserUseCase,
    ChangePasswordUseCase changePasswordUseCase) : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase = createUserUseCase;
    private readonly GetUserByIdUseCase _getUserByIdUseCase = getUserByIdUseCase;
    private readonly GetUsersPagedUseCase _getUsersPagedUseCase = getUsersPagedUseCase;
    private readonly UpdateUserUseCase _updateUserUseCase = updateUserUseCase;
    private readonly DeleteUserUseCase _deleteUserUseCase = deleteUserUseCase;
    private readonly ChangePasswordUseCase _changePasswordUseCase = changePasswordUseCase;

    /// <summary>
    /// Cadastra um novo usuário no sistema.
    /// </summary>
    [HttpPost]
    [AllowAnonymous] // Permite autorregistro inicial ou criação de conta
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserInput request,
        CancellationToken cancellationToken)
    {
        var result = await _createUserUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result.Id }, result);
    }

    /// <summary>
    /// Lista usuários paginados (Apenas Admin).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _getUsersPagedUseCase.ExecuteAsync(pageIndex, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém detalhes de um usuário por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getUserByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza dados de um usuário (Email e Role).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUserInput request,
        CancellationToken cancellationToken)
    {
        var result = await _updateUserUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Remove logicamente (Soft Delete) um usuário (Apenas Admin).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await _deleteUserUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Altera a senha do usuário.
    /// </summary>
    [HttpPut("{id:guid}/password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        [FromRoute] Guid id,
        [FromBody] ChangePasswordInput request,
        CancellationToken cancellationToken)
    {
        await _changePasswordUseCase.ExecuteAsync(id, request, cancellationToken);
        return NoContent();
    }
}
