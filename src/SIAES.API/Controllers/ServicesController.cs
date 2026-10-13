using Application.UseCases.Services;
using Application.UseCases.Services.Dto;
using Application.UseCases.Users;
using Application.UseCases.Users.Dto;
using Asp.Versioning;
using Domain.Common.Paginate;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ServicesController(
        CreateServicesUseCase createServicesUseCase,
        UpdateServiceUserCase updateServiceUserCase,
        DeleteServiceUserCase deleteServiceUserCase,
        GetServiceByIdUserCase getServiceByIdUserCase,
        GetServicesPagedUseCase getServicesPagedUseCase) : ControllerBase
    {
        private readonly CreateServicesUseCase _createServicesUseCase = createServicesUseCase;
        private readonly UpdateServiceUserCase _updateServiceUserCase = updateServiceUserCase;
        private readonly DeleteServiceUserCase _deleteServiceUserCase = deleteServiceUserCase;
        private readonly GetServiceByIdUserCase _getServiceByIdUserCase = getServiceByIdUserCase;   
        private readonly GetServicesPagedUseCase _getServicesPagedUseCase = getServicesPagedUseCase;

        /// <summary>
        /// Cadastra um novo serviço no sistema.
        /// </summary>
        /// <param name="request">Os dados do serviço a ser cadastrado</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>Os dados do serviço cadastrado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateServiceInput request, CancellationToken cancellationToken)
        {
            var result = await _createServicesUseCase.ExecuteAsync(request, cancellationToken);

            //return CreatedAtAction(nameof(GetById), new { version = "1.0", id = result. }, result);
            return CreatedAtAction(nameof(GetById), new { version = "1.0", id = 1 }, result);
        }

        /// <summary>
        /// Atualiza dados de um serviço (Nome do Serviço).
        /// </summary>
        /// <param name="idService">O ID do serviço</param>
        /// <param name="request">Os dados do serviço para atualização</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>Os dados atualizados do serviço</returns>
        [HttpPut("{idService:guid}")]
        [ProducesResponseType(typeof(ServiceOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid idService,[FromBody] UpdateServiceInput request,CancellationToken cancellationToken)
        {
            ServiceOutput result = await _updateServiceUserCase.ExecuteAsync(idService, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Remove logicamente (Soft Delete) um serviço.
        /// </summary>
        /// <param name="idService">O ID do serviço</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>No Content</returns>
        [HttpDelete("{idService:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid idService,CancellationToken cancellationToken)
        {
            await _deleteServiceUserCase.ExecuteAsync(idService, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Obtém os detalhes de um serviço específico pelo seu ID.
        /// </summary>
        /// <param name="idService">O ID do serviço</param>
        /// <param name="cancellationToken">O token de cancelamento</param>
        /// <returns>Os dados do serviço</returns>
        [HttpGet("{idService:guid}")]
        [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid idService ,CancellationToken cancellationToken)
        {
            ServiceOutput result = await _getServiceByIdUserCase.ExecuteAsync(idService, cancellationToken);
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
            IPaginate<ServiceOutput> result = await _getServicesPagedUseCase.ExecuteAsync(pageIndex, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}
