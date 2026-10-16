using Application.UseCases.Servicos;
using Application.UseCases.Servicos.Dto;
using Asp.Versioning;
using Domain.Common.Paginate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ServicosController(
        CreateServicoUserCase createServicosUseCase,
        UpdateServicoUserCase updateServicoUserCase,
        DeleteServicoUserCase deleteServicoUserCase,
        GetServicoByIdUserCase getServicoByIdUserCase,
        GetServicoPagedUseCase getServicosPagedUseCase) : ControllerBase
    {
        private readonly CreateServicoUserCase _createServicosUseCase = createServicosUseCase;
        private readonly UpdateServicoUserCase _updateServicoUserCase = updateServicoUserCase;
        private readonly DeleteServicoUserCase _deleteServicoUserCase = deleteServicoUserCase;
        private readonly GetServicoByIdUserCase _getServicoByIdUserCase = getServicoByIdUserCase;   
        private readonly GetServicoPagedUseCase _getServicosPagedUseCase = getServicosPagedUseCase;

        /// <summary>
        /// Cadastra um novo serviço no sistema.
        /// </summary>
        /// <param name="request">Os dados do serviço a ser cadastrado</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>Os dados do serviço cadastrado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServicoOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateServicoInput request, CancellationToken cancellationToken)
        {
            var result = await _createServicosUseCase.ExecuteAsync(request, cancellationToken);

            //return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result. }, result);
            return CreatedAtAction(nameof(GetById), new { version = "1.0", id = 1 }, result);
        }

        /// <summary>
        /// Atualiza dados de um serviço (Nome do Serviço).
        /// </summary>
        /// <param name="id">O ID do serviço</param>
        /// <param name="request">Os dados do serviço para atualização</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>Os dados atualizados do serviço</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ServicoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody] UpdateServicoInput request,CancellationToken cancellationToken)
        {
            ServicoOutput result = await _updateServicoUserCase.ExecuteAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Remove logicamente (Soft Delete) um serviço.
        /// </summary>
        /// <param name="id">O ID do serviço</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>No Content</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            await _deleteServicoUserCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Obtém os detalhes de um serviço específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do serviço</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>Os dados do serviço</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ServicoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            ServicoOutput result = await _getServicoByIdUserCase.ExecuteAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Lista serviços cadastrados de forma paginada 
        /// </summary>
        /// <param name="pageIndex">O índice da página</param>
        /// <param name="pageSize">O tamanho da página</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>Uma lista de serviços cadastrados e não excluídos de forma paginados</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 0,[FromQuery] int pageSize = 10,CancellationToken cancellationToken = default)
        {
            IPaginate<ServicoOutput> result = await _getServicosPagedUseCase.ExecuteAsync(pageIndex, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}
