using System.Net;
using System.Net.Http.Json;
using Application.UseCases.OrdensServico.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using IntegrationTests.Common;

namespace IntegrationTests.Controllers;

public class OrdensServicoControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _authClient;
    private readonly HttpClient _userClient;
    private readonly HttpClient _anonymousClient;

    public OrdensServicoControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _authClient = factory.CreateClientWithRole(Roles.Operacional);
        _userClient = factory.CreateClientWithRole(Roles.User);
        _anonymousClient = factory.CreateClient();
    }

    private (Cliente cliente, Veiculo veiculo) SetupClienteEVeiculo(string cpf = "52998224725", string placa = "ABC1D23")
    {
        var cliente = new Cliente("Cliente OS", new Documento(cpf), "os@email.com", "11999998888",
            new Endereco("Rua", "1", null, "Bairro", "Cidade", "SP", "01001000"));
        _factory.ClienteRepository.Seed(cliente);

        var veiculo = new Veiculo(cliente.Id, new Placa(placa), "Toyota", "Corolla", 2022);
        _factory.VeiculoRepository.Seed(veiculo);

        return (cliente, veiculo);
    }

    [Fact]
    public async Task ObterStatusPorId_AnonimoEOsExistente_DeveRetornar200OK()
    {
        // Arrange
        var (cliente, veiculo) = SetupClienteEVeiculo();
        var os = new OrdemServico(cliente.Id, veiculo.Id);
        _factory.OrdemServicoRepository.Seed(os);

        // Act
        var response = await _anonymousClient.GetAsync($"/api/v1/ordens-servico/{os.Id}/status");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<GetStatusClienteResponse>();
        Assert.NotNull(content);
        Assert.Equal(os.Id, content.OrdemServicoId);
        Assert.Equal("Recebida", content.Status);
    }

    [Fact]
    public async Task ObterStatusPorId_OsInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _anonymousClient.GetAsync($"/api/v1/ordens-servico/{Guid.NewGuid()}/status");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ObterStatusPorDocumento_AnonimoEDocumentoValido_DeveRetornar200OK()
    {
        // Arrange
        var cpf = "52998224725";
        var (cliente, veiculo) = SetupClienteEVeiculo(cpf);
        var os = new OrdemServico(cliente.Id, veiculo.Id);
        _factory.OrdemServicoRepository.Seed(os);

        // Act
        var response = await _anonymousClient.GetAsync($"/api/v1/ordens-servico/documento/{cpf}/status");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<List<GetStatusClienteResponse>>();
        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task ObterStatusPorDocumento_DocumentoInvalido_DeveRetornar400BadRequest()
    {
        // Act
        var response = await _anonymousClient.GetAsync("/api/v1/ordens-servico/documento/123456/status");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Criar_ComDadosValidos_DeveRetornar201CreatedComLocation()
    {
        // Arrange
        var (cliente, veiculo) = SetupClienteEVeiculo(placa: "KTM9Z88");
        var request = new CreateOrdemServicoInput(cliente.Id, veiculo.Id);

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/ordens-servico", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var content = await response.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.NotNull(content);
        Assert.Equal("Recebida", content.Status);
    }

    [Fact]
    public async Task Criar_ClienteInexistente_DeveRetornar404NotFound()
    {
        // Arrange
        var request = new CreateOrdemServicoInput(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var response = await _authClient.PostAsJsonAsync("/api/v1/ordens-servico", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Criar_Anonimo_DeveRetornar401Unauthorized()
    {
        // Arrange
        var request = new CreateOrdemServicoInput(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var response = await _anonymousClient.PostAsJsonAsync("/api/v1/ordens-servico", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Criar_PerfilUser_DeveRetornar403Forbidden()
    {
        // Arrange
        var request = new CreateOrdemServicoInput(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var response = await _userClient.PostAsJsonAsync("/api/v1/ordens-servico", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ObterPorId_OsExistente_DeveRetornar200OK()
    {
        // Arrange
        var (cliente, veiculo) = SetupClienteEVeiculo(placa: "ZTR1A22");
        var os = new OrdemServico(cliente.Id, veiculo.Id);
        _factory.OrdemServicoRepository.Seed(os);

        // Act
        var response = await _authClient.GetAsync($"/api/v1/ordens-servico/{os.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.NotNull(content);
        Assert.Equal(os.Id, content.Id);
    }

    [Fact]
    public async Task ObterPorId_OsInexistente_DeveRetornar404NotFound()
    {
        // Act
        var response = await _authClient.GetAsync($"/api/v1/ordens-servico/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Listar_DeveRetornar200OK()
    {
        // Act
        var response = await _authClient.GetAsync("/api/v1/ordens-servico?pageIndex=0&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CicloDeVidaCompletoDaOs_DeveTransitarPorTodosOsStatusCorretamente()
    {
        // Arrange
        var (cliente, veiculo) = SetupClienteEVeiculo(placa: "CIC1L01");
        var itemEstoque = new ItemEstoque("FIL-01", "Filtro", null, UnidadeMedida.Unidade, 80m);
        itemEstoque.AdicionarEstoque(10);
        _factory.ItemEstoqueRepository.Seed(itemEstoque);

        var servicoCatalogo = new Servico("Mão de Obra Troca Filtro");
        _factory.ServicoRepository.Seed(servicoCatalogo);

        var os = new OrdemServico(cliente.Id, veiculo.Id);
        _factory.OrdemServicoRepository.Seed(os);

        // 1. Iniciar Diagnóstico (Recebida -> EmDiagnostico)
        var diagResponse = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/diagnostico", null);
        Assert.Equal(HttpStatusCode.OK, diagResponse.StatusCode);
        var diagOutput = await diagResponse.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.Equal("EmDiagnostico", diagOutput!.Status);

        // 2. Adicionar Peça
        var pecaInput = new AdicionarPecaInput(itemEstoque.Id, 1, 80m);
        var pecaResponse = await _authClient.PostAsJsonAsync($"/api/v1/ordens-servico/{os.Id}/pecas", pecaInput);
        Assert.Equal(HttpStatusCode.OK, pecaResponse.StatusCode);

        // 3. Adicionar Serviço
        var servicoInput = new AdicionarServicoInput(servicoCatalogo.Id, "Troca de filtro", 50m, 1);
        var servicoResponse = await _authClient.PostAsJsonAsync($"/api/v1/ordens-servico/{os.Id}/servicos", servicoInput);
        Assert.Equal(HttpStatusCode.OK, servicoResponse.StatusCode);

        // 4. Gerar Orçamento (EmDiagnostico -> AguardandoAprovacao)
        var orcResponse = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/orcamento", null);
        Assert.Equal(HttpStatusCode.OK, orcResponse.StatusCode);
        var orcOutput = await orcResponse.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.Equal("AguardandoAprovacao", orcOutput!.Status);

        // 5. Aprovar Orçamento (AguardandoAprovacao -> EmExecucao)
        var aprResponse = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/aprovar", null);
        Assert.Equal(HttpStatusCode.OK, aprResponse.StatusCode);
        var aprOutput = await aprResponse.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.Equal("EmExecucao", aprOutput!.Status);

        // 6. Solicitar Aprovação Adicional (EmExecucao -> AguardandoAprovacao)
        var addResponse = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/solicitar-aprovacao-adicional", null);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        var addOutput = await addResponse.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.Equal("AguardandoAprovacao", addOutput!.Status);

        // 7. Re-Aprovar Orçamento (AguardandoAprovacao -> EmExecucao)
        var reaprovarResponse = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/aprovar", null);
        Assert.Equal(HttpStatusCode.OK, reaprovarResponse.StatusCode);

        // 8. Finalizar (EmExecucao -> Finalizada)
        var finResponse = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/finalizar", null);
        Assert.Equal(HttpStatusCode.OK, finResponse.StatusCode);
        var finOutput = await finResponse.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.Equal("Finalizada", finOutput!.Status);

        // 9. Entregar (Finalizada -> Entregue)
        var entResponse = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/entregar", null);
        Assert.Equal(HttpStatusCode.OK, entResponse.StatusCode);
        var entOutput = await entResponse.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.Equal("Entregue", entOutput!.Status);
    }

    [Fact]
    public async Task RejeitarOrcamento_AguardandoAprovacao_DeveMudarStatusParaCancelada()
    {
        // Arrange
        var (cliente, veiculo) = SetupClienteEVeiculo(placa: "REJ1A99");
        var os = new OrdemServico(cliente.Id, veiculo.Id);
        _factory.OrdemServicoRepository.Seed(os);

        await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/diagnostico", null);

        var servicoCatalogo = new Servico("Mão de Obra Teste Rejeição");
        _factory.ServicoRepository.Seed(servicoCatalogo);
        var servicoInput = new AdicionarServicoInput(servicoCatalogo.Id, "Mão de Obra Teste Rejeição", 50m, 1);
        await _authClient.PostAsJsonAsync($"/api/v1/ordens-servico/{os.Id}/servicos", servicoInput);

        await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/orcamento", null);

        // Act
        var response = await _authClient.PostAsync($"/api/v1/ordens-servico/{os.Id}/rejeitar", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<OrdemServicoOutput>();
        Assert.NotNull(output);
        Assert.Equal("Cancelada", output.Status);
    }
}

