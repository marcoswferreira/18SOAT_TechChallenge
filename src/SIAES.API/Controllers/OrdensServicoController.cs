using Application.UseCases.OrdensServico;
using Application.UseCases.OrdensServico.Dto;
using Asp.Versioning;
using Domain.Common.Paginate;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIAES.API.Controllers;

/// <summary>
/// Ciclo de vida da Ordem de Serviço. Por padrão, todas as operações exigem autenticação
/// e um perfil operacional; apenas as duas consultas de status são públicas (<see cref="AllowAnonymousAttribute"/>).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ordens-servico")]
[Authorize(Roles = PerfisOperacao)]
public class OrdensServicoController(
    GetStatusOrdemServicoUseCase getStatusUseCase,
    CreateOrdemServicoUseCase createUseCase,
    GetOrdemServicoByIdUseCase getByIdUseCase,
    GetOrdensServicoPagedUseCase getPagedUseCase,
    IniciarDiagnosticoUseCase iniciarDiagnosticoUseCase,
    AdicionarPecaOrdemServicoUseCase adicionarPecaUseCase,
    AdicionarServicoOrdemServicoUseCase adicionarServicoUseCase,
    GerarOrcamentoUseCase gerarOrcamentoUseCase,
    SolicitarAprovacaoAdicionalUseCase solicitarAprovacaoAdicionalUseCase,
    AprovarOrcamentoUseCase aprovarOrcamentoUseCase,
    RejeitarOrcamentoUseCase rejeitarOrcamentoUseCase,
    FinalizarOrdemServicoUseCase finalizarOrdemServicoUseCase,
    EntregarVeiculoUseCase entregarVeiculoUseCase) : ControllerBase
{
    /// <summary>Perfis que operam Ordens de Serviço (atendente e mecânico compartilham <see cref="Roles.Operacional"/>).</summary>
    private const string PerfisOperacao = $"{Roles.Admin},{Roles.Manager},{Roles.Operacional}";

    /// <summary>Consulta pública do status de uma OS pelo ID.</summary>
    [HttpGet("{id:guid}/status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetStatusClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterStatusPorId(Guid id, CancellationToken ct)
    {
        var resultado = await getStatusUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Consulta pública de todas as OS de um cliente pelo CPF ou CNPJ.</summary>
    [HttpGet("documento/{documento}/status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<GetStatusClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterStatusPorDocumento(string documento, CancellationToken ct)
    {
        var resultado = await getStatusUseCase.ExecuteAsync(documento, ct);
        return Ok(resultado);
    }

    /// <summary>Cria uma nova ordem de serviço (status inicial: Recebida).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateOrdemServicoInput input, CancellationToken ct)
    {
        var resultado = await createUseCase.ExecuteAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = resultado.Id }, resultado);
    }

    /// <summary>Obtém os detalhes completos de uma OS pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var resultado = await getByIdUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Lista ordens de serviço de forma paginada.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IPaginate<OrdemServicoOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var resultado = await getPagedUseCase.ExecuteAsync(pageIndex, pageSize, ct);
        return Ok(resultado);
    }

    /// <summary>Inicia o diagnóstico técnico da OS (status: EmDiagnostico).</summary>
    [HttpPost("{id:guid}/diagnostico")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> IniciarDiagnostico(Guid id, CancellationToken ct)
    {
        var resultado = await iniciarDiagnosticoUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Adiciona uma peça à OS e reserva saldo no estoque.</summary>
    [HttpPost("{id:guid}/pecas")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarPeca(Guid id, [FromBody] AdicionarPecaInput input, CancellationToken ct)
    {
        var resultado = await adicionarPecaUseCase.ExecuteAsync(id, input, ct);
        return Ok(resultado);
    }

    /// <summary>Adiciona um serviço/mão de obra à OS.</summary>
    [HttpPost("{id:guid}/servicos")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarServico(Guid id, [FromBody] AdicionarServicoInput input, CancellationToken ct)
    {
        var resultado = await adicionarServicoUseCase.ExecuteAsync(id, input, ct);
        return Ok(resultado);
    }

    /// <summary>Gera e envia o orçamento ao cliente (status: AguardandoAprovacao).</summary>
    [HttpPost("{id:guid}/orcamento")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GerarOrcamento(Guid id, CancellationToken ct)
    {
        var resultado = await gerarOrcamentoUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Solicita aprovação adicional de orçamento para a OS em execução (retorna para AguardandoAprovacao).</summary>
    [HttpPost("{id:guid}/solicitar-aprovacao-adicional")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SolicitarAprovacaoAdicional(Guid id, CancellationToken ct)
    {
        var resultado = await solicitarAprovacaoAdicionalUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Aprova o orçamento e inicia a execução (status: EmExecucao).</summary>
    [HttpPost("{id:guid}/aprovar")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Aprovar(Guid id, CancellationToken ct)
    {
        var resultado = await aprovarOrcamentoUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Rejeita o orçamento, cancela a OS e libera a reserva das peças no estoque.</summary>
    [HttpPost("{id:guid}/rejeitar")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rejeitar(Guid id, CancellationToken ct)
    {
        var resultado = await rejeitarOrcamentoUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Finaliza os serviços da OS e dá baixa definitiva no estoque físico.</summary>
    [HttpPost("{id:guid}/finalizar")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Finalizar(Guid id, CancellationToken ct)
    {
        var resultado = await finalizarOrdemServicoUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }

    /// <summary>Registra a entrega do veículo ao cliente (status: Entregue).</summary>
    [HttpPost("{id:guid}/entregar")]
    [ProducesResponseType(typeof(OrdemServicoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Entregar(Guid id, CancellationToken ct)
    {
        var resultado = await entregarVeiculoUseCase.ExecuteAsync(id, ct);
        return Ok(resultado);
    }
}