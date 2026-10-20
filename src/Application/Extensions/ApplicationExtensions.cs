using Application.UseCases.Auth;
using Application.UseCases.Clientes;
using Application.UseCases.ItensEstoque;
using Application.UseCases.OrdensServico;
using Application.UseCases.Servicos;
using Application.UseCases.Users;
using Application.UseCases.Veiculos;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Auth Use Cases
        services.AddScoped<AuthenticateUserUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<RevokeTokenUseCase>();

        // User Use Cases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<GetUsersPagedUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<ChangePasswordUseCase>();

        // Cliente Use Cases
        services.AddScoped<CreateClienteUseCase>();
        services.AddScoped<GetClienteByIdUseCase>();
        services.AddScoped<GetClientesPagedUseCase>();
        services.AddScoped<UpdateClienteUseCase>();
        services.AddScoped<DeleteClienteUseCase>();

        // Veiculo Use Cases
        services.AddScoped<CreateVeiculoUseCase>();
        services.AddScoped<GetVeiculoByIdUseCase>();
        services.AddScoped<GetVeiculosPagedUseCase>();
        services.AddScoped<UpdateVeiculoUseCase>();
        services.AddScoped<DeleteVeiculoUseCase>();

        // ItemEstoque Use Cases
        services.AddScoped<CreateItemEstoqueUseCase>();
        services.AddScoped<GetItemEstoqueByIdUseCase>();
        services.AddScoped<GetItensEstoquePagedUseCase>();
        services.AddScoped<UpdateItemEstoqueUseCase>();
        services.AddScoped<DeleteItemEstoqueUseCase>();
        services.AddScoped<RegistrarEntradaEstoqueUseCase>();

        // Serviços Use Cases
        services.AddScoped<CreateServicoUserCase>();
        services.AddScoped<UpdateServicoUserCase>();
        services.AddScoped<DeleteServicoUserCase>();
        services.AddScoped<GetServicoByIdUserCase>();
        services.AddScoped<GetServicoPagedUseCase>();

        // Ordem de Serviço Use Cases
        services.AddScoped<GetStatusOrdemServicoUseCase>();
        services.AddScoped<CreateOrdemServicoUseCase>();
        services.AddScoped<GetOrdemServicoByIdUseCase>();
        services.AddScoped<GetOrdensServicoPagedUseCase>();
        services.AddScoped<IniciarDiagnosticoUseCase>();
        services.AddScoped<AdicionarPecaOrdemServicoUseCase>();
        services.AddScoped<AdicionarServicoOrdemServicoUseCase>();
        services.AddScoped<GerarOrcamentoUseCase>();
        services.AddScoped<SolicitarAprovacaoAdicionalUseCase>();
        services.AddScoped<AprovarOrcamentoUseCase>();
        services.AddScoped<RejeitarOrcamentoUseCase>();
        services.AddScoped<FinalizarOrdemServicoUseCase>();
        services.AddScoped<EntregarVeiculoUseCase>();

        return services;
    }
}
