using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueObjects;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, ILogger logger)
    {
        await SeedUsersAsync(dbContext, passwordHasher, logger);
        await SeedClientesEVeiculosAsync(dbContext, logger);
        await SeedItensEstoqueAsync(dbContext, logger);
        await SeedServicosAsync(dbContext, logger);
    }

    private static async Task SeedUsersAsync(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, ILogger logger)
    {
        try
        {
            var defaultUsers = new (string Email, string Password, string[] Roles)[]
            {
                ("admin@siaes.com", "SenhaAdmin1!", [Roles.Admin]),
                ("manager@siaes.com", "SenhaManager1!", [Roles.Manager]),
                ("atendente@siaes.com", "SenhaAten1!", [Roles.Operacional]),
                ("mecanico@siaes.com", "SenhaMecanico1!", [Roles.Operacional]),
                ("usuario@siaes.com", "SenhaUsuario1!", [Roles.User])
            };

            var usersAdded = 0;
            var rolesAdded = 0;

            foreach (var (email, password, roles) in defaultUsers)
            {
                var normalizedEmail = email.Trim().ToLowerInvariant();

                var existingUser = await dbContext.Users
                    .Include(u => u.Roles)
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

                if (existingUser is null)
                {
                    // Usuário não existe: cria com roles
                    var passwordHash = passwordHasher.Hash(password);
                    var user = new User(normalizedEmail, passwordHash, roles);
                    await dbContext.Users.AddAsync(user);
                    usersAdded++;
                }
                else if (existingUser.Roles.Count == 0)
                {
                    // Usuário existe mas sem roles: adiciona as roles padrão
                    foreach (var role in roles)
                        existingUser.AddRole(role);
                    rolesAdded++;
                }
            }

            if (usersAdded > 0 || rolesAdded > 0)
            {
                await dbContext.SaveChangesAsync();

                if (usersAdded > 0)
                    logger.LogInformation("DatabaseSeeder: Seeded {Count} initial user(s) successfully.", usersAdded);

                if (rolesAdded > 0)
                    logger.LogInformation("DatabaseSeeder: Seeded roles for {Count} existing user(s) without roles.", rolesAdded);
            }
            else
            {
                logger.LogInformation("DatabaseSeeder: Default users already exist in database.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DatabaseSeeder: An error occurred while seeding default users.");
            throw;
        }
    }

    private static async Task SeedClientesEVeiculosAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        try
        {
            var seedData = new (
                string Nome,
                string Documento,
                string? Email,
                string? Telefone,
                Endereco Endereco,
                (string Placa, string Marca, string Modelo, int Ano) Veiculo
            )[]
            {
                (
                    "Carlos Eduardo Silva",
                    "12345678909",
                    "carlos.silva@email.com",
                    "(11) 98765-4321",
                    new Endereco("Avenida Paulista", "1000", "Apto 101", "Bela Vista", "São Paulo", "SP", "01310100"),
                    ("BRA2E19", "Toyota", "Corolla XEi", 2022)
                ),
                (
                    "Mariana Souza Oliveira",
                    "98765432100",
                    "mariana.souza@email.com",
                    "(41) 99887-1122",
                    new Endereco("Rua das Flores", "250", null, "Centro", "Curitiba", "PR", "80010000"),
                    ("ABC1234", "Honda", "Civic Touring", 2021)
                ),
                (
                    "Lucas Ferreira Santos",
                    "11144477735",
                    "lucas.santos@email.com",
                    "(21) 97654-3210",
                    new Endereco("Avenida Atlântica", "1500", "Bloco B", "Copacabana", "Rio de Janeiro", "RJ", "22021001"),
                    ("RIO2A18", "Volkswagen", "Golf GTI", 2023)
                ),
                (
                    "Juliana Mendes Ribeiro",
                    "22255588846",
                    "juliana.mendes@email.com",
                    "(31) 98123-4567",
                    new Endereco("Rua da Bahia", "800", "Sala 302", "Lourdes", "Belo Horizonte", "MG", "30160011"),
                    ("SPX9876", "Chevrolet", "Onix Premier", 2020)
                ),
                (
                    "Transportes & Logística Brasil Ltda",
                    "11222333000181",
                    "contato@transbrasil.com.br",
                    "(81) 3456-7890",
                    new Endereco("Avenida Boa Viagem", "3200", null, "Boa Viagem", "Recife", "PE", "51020000"),
                    ("KXZ3B45", "Volvo", "FH 540", 2024)
                )
            };

            var allClientes = await dbContext.Clientes
                .Include(c => c.Veiculos)
                .IgnoreQueryFilters()
                .ToListAsync();

            var clientesAdicionados = 0;
            var veiculosAdicionados = 0;

            foreach (var item in seedData)
            {
                var documentoVo = new Documento(item.Documento);
                var existingCliente = allClientes.FirstOrDefault(c => c.Documento == documentoVo);

                if (existingCliente is null)
                {
                    var cliente = new Cliente(
                        item.Nome,
                        documentoVo,
                        item.Email,
                        item.Telefone,
                        item.Endereco
                    );

                    var veiculo = new Veiculo(
                        cliente.Id,
                        new Placa(item.Veiculo.Placa),
                        item.Veiculo.Marca,
                        item.Veiculo.Modelo,
                        item.Veiculo.Ano
                    );

                    cliente.AdicionarVeiculo(veiculo);
                    await dbContext.Clientes.AddAsync(cliente);
                    allClientes.Add(cliente);
                    clientesAdicionados++;
                    veiculosAdicionados++;
                }
                else
                {
                    var placaVo = new Placa(item.Veiculo.Placa);
                    var jaPossuiVeiculo = existingCliente.Veiculos.Any(v => v.Placa == placaVo);

                    if (!jaPossuiVeiculo)
                    {
                        var veiculoJaExisteNoBanco = await dbContext.Veiculos
                            .IgnoreQueryFilters()
                            .AnyAsync(v => v.Placa == placaVo);

                        if (!veiculoJaExisteNoBanco)
                        {
                            var veiculo = new Veiculo(
                                existingCliente.Id,
                                placaVo,
                                item.Veiculo.Marca,
                                item.Veiculo.Modelo,
                                item.Veiculo.Ano
                            );

                            existingCliente.AdicionarVeiculo(veiculo);
                            veiculosAdicionados++;
                        }
                    }
                }
            }

            if (clientesAdicionados > 0 || veiculosAdicionados > 0)
            {
                await dbContext.SaveChangesAsync();
                logger.LogInformation(
                    "DatabaseSeeder: Seeded {ClientesCount} cliente(s) e {VeiculosCount} veiculo(s) com sucesso.",
                    clientesAdicionados,
                    veiculosAdicionados
                );
            }
            else
            {
                logger.LogInformation("DatabaseSeeder: Clientes e veículos padrão já existem no banco de dados.");
            }
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "DatabaseSeeder: Conflito de dados ao semear clientes e veículos padrão. A inicialização da aplicação prosseguirá normalmente.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DatabaseSeeder: Ocorreu um erro ao semear clientes e veículos padrão. A inicialização da aplicação prosseguirá normalmente.");
        }
    }

    private static async Task SeedItensEstoqueAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        try
        {
            var seedData = new (string Codigo, string Nome, string? Descricao, UnidadeMedida Unidade, decimal Preco, decimal Estoque)[]
            {
                ("FLT-OLEO-001", "Filtro de óleo", "Filtro de óleo para motores 1.0 a 2.0", UnidadeMedida.Unidade, 35.90m, 20),
                ("FLT-AR-001", "Filtro de ar do motor", null, UnidadeMedida.Unidade, 42.50m, 15),
                ("FLT-CAB-001", "Filtro de ar condicionado", "Filtro de cabine", UnidadeMedida.Unidade, 38.00m, 10),
                ("PST-FREIO-D01", "Pastilha de freio dianteira", "Jogo com 4 pastilhas", UnidadeMedida.Unidade, 129.90m, 8),
                ("DSC-FREIO-D01", "Disco de freio dianteiro", null, UnidadeMedida.Unidade, 189.90m, 4),
                ("VELA-IGN-001", "Vela de ignição", null, UnidadeMedida.Unidade, 24.90m, 32),
                ("BAT-60AH", "Bateria 60Ah", null, UnidadeMedida.Unidade, 489.00m, 3),
                ("OLEO-5W30", "Óleo de motor 5W30 sintético", null, UnidadeMedida.Litro, 49.90m, 60),
                ("FLUIDO-FREIO-DOT4", "Fluido de freio DOT 4", null, UnidadeMedida.Litro, 39.90m, 12.5m),
                ("ADIT-RAD-001", "Aditivo para radiador", "Concentrado, diluir em água", UnidadeMedida.Litro, 29.90m, 18),
                ("GRAXA-001", "Graxa multiuso", null, UnidadeMedida.Quilo, 54.00m, 5)
            };

            var itensAdicionados = 0;

            foreach (var item in seedData)
            {
                var jaExiste = await dbContext.ItensEstoque
                    .IgnoreQueryFilters()
                    .AnyAsync(i => i.Codigo == item.Codigo);

                if (jaExiste)
                    continue;

                var itemEstoque = new ItemEstoque(item.Codigo, item.Nome, item.Descricao, item.Unidade, item.Preco);
                itemEstoque.AdicionarEstoque(item.Estoque);

                await dbContext.ItensEstoque.AddAsync(itemEstoque);
                itensAdicionados++;
            }

            if (itensAdicionados > 0)
            {
                await dbContext.SaveChangesAsync();
                logger.LogInformation("DatabaseSeeder: Seeded {Count} item(ns) de estoque com sucesso.", itensAdicionados);
            }
            else
            {
                logger.LogInformation("DatabaseSeeder: Itens de estoque padrão já existem no banco de dados.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DatabaseSeeder: An error occurred while seeding default itens de estoque.");
            throw;
        }
    }

    private static async Task SeedServicosAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        try
        {
            var defaultServicos = new (Guid IdService, string ServiceName)[]
            {
                (Guid.NewGuid(), "Troca de óleo e filtros"),
                (Guid.NewGuid(), "Revisão periódica"),
                (Guid.NewGuid(), "Alinhamento e balanceamento"),
                (Guid.NewGuid(), "Rodízio de pneus"),
                (Guid.NewGuid(), "Substituição de fluidos"),
                (Guid.NewGuid(), "Verificação de correias"),
                (Guid.NewGuid(), "Troca de pastilhas e discos de freio"),
                (Guid.NewGuid(), "Reparo e sangria do sistema hidráulico"),
                (Guid.NewGuid(), "Manutenção do freio de estacionamento"),
                (Guid.NewGuid(), "Troca de amortecedores e batentes"),
                (Guid.NewGuid(), "Substituição de molas e bandejas de suspensão"),
                (Guid.NewGuid(), "Diagnóstico computadorizado"),
                (Guid.NewGuid(), "Limpeza de bicos injetores"),
                (Guid.NewGuid(), "Manutenção de alternadores e motores de partida (de arranque)"),
                (Guid.NewGuid(), "Reparo no sistema de iluminação"),

            };

            var servicosAdded = 0;

            foreach (var (id, nome) in defaultServicos)
            {
                var existingServico = await dbContext.Servicos
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Nome == nome);

                if (existingServico is null)
                {
                    var itemServico = new Servico(id, nome);
                    await dbContext.Servicos.AddAsync(itemServico);
                    servicosAdded++;
                }
            }

            if (servicosAdded > 0)
            {
                await dbContext.SaveChangesAsync();

                logger.LogInformation("DatabaseSeeder: Seeded {Count} initial servico(s) successfully.", servicosAdded);

            }
            else
            {
                logger.LogInformation("DatabaseSeeder: Default servicos already exist in database.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DatabaseSeeder: An error occurred while seeding default servicos.");
            throw;
        }
    }
}
