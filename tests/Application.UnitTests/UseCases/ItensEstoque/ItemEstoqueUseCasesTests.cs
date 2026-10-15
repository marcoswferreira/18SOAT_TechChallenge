using Application.UseCases.ItensEstoque;
using Application.UseCases.ItensEstoque.Dto;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.ItensEstoque;

public class ItemEstoqueUseCasesTests
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = Substitute.For<IItemEstoqueRepository>();

    private static ItemEstoque CriarItem(decimal estoque = 0)
    {
        var item = new ItemEstoque("PST-01", "Pastilha de freio", null, UnidadeMedida.Unidade, 120m);
        if (estoque > 0)
            item.AdicionarEstoque(estoque);
        return item;
    }

    private static CreateItemEstoqueInput CriarInput() =>
        new("PST-01", "Pastilha de freio", null, UnidadeMedida.Unidade, 120m);

    // -------------------------------------------------------------------------
    // Create
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Create_WithValidInput_ShouldAddItemComEstoqueZerado()
    {
        var useCase = new CreateItemEstoqueUseCase(_itemEstoqueRepository);

        var result = await useCase.ExecuteAsync(CriarInput(), CancellationToken.None);

        Assert.Equal("PST-01", result.Codigo);
        Assert.Equal(0, result.QuantidadeEmEstoque);
        await _itemEstoqueRepository.Received(1).AddAsync(Arg.Any<ItemEstoque>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WithExistingCodigo_ShouldThrowInvalidOperationException()
    {
        _itemEstoqueRepository.ExistsByCodigoAsync("PST-01", Arg.Any<CancellationToken>()).Returns(true);
        var useCase = new CreateItemEstoqueUseCase(_itemEstoqueRepository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(CriarInput(), CancellationToken.None));

        await _itemEstoqueRepository.DidNotReceive().AddAsync(Arg.Any<ItemEstoque>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WithoutCodigo_ShouldThrowArgumentException()
    {
        var useCase = new CreateItemEstoqueUseCase(_itemEstoqueRepository);
        var input = CriarInput() with { Codigo = " " };

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(input, CancellationToken.None));
    }

    // -------------------------------------------------------------------------
    // Get
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetById_Existing_ShouldReturnItem()
    {
        var item = CriarItem(estoque: 3);
        _itemEstoqueRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var useCase = new GetItemEstoqueByIdUseCase(_itemEstoqueRepository);

        var result = await useCase.ExecuteAsync(item.Id, CancellationToken.None);

        Assert.Equal(item.Id, result.Id);
        Assert.Equal(3, result.QuantidadeDisponivel);
    }

    [Fact]
    public async Task GetById_NotFound_ShouldThrowKeyNotFoundException()
    {
        _itemEstoqueRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ItemEstoque?)null);
        var useCase = new GetItemEstoqueByIdUseCase(_itemEstoqueRepository);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetPaged_ShouldMapItems()
    {
        var paged = Substitute.For<IPaginate<ItemEstoque>>();
        paged.Items.Returns([CriarItem()]);
        paged.Count.Returns(1);
        _itemEstoqueRepository.GetPagedAsync(0, 10, Arg.Any<CancellationToken>()).Returns(paged);
        var useCase = new GetItensEstoquePagedUseCase(_itemEstoqueRepository);

        var result = await useCase.ExecuteAsync(0, 10, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("PST-01", result.Items[0].Codigo);
        Assert.Equal(1, result.Count);
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Update_Existing_ShouldUpdateData()
    {
        var item = CriarItem(estoque: 4);
        _itemEstoqueRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var useCase = new UpdateItemEstoqueUseCase(_itemEstoqueRepository);
        var input = new UpdateItemEstoqueInput("Pastilha cerâmica", "Dianteira", UnidadeMedida.Unidade, 150m);

        var result = await useCase.ExecuteAsync(item.Id, input, CancellationToken.None);

        Assert.Equal("Pastilha cerâmica", result.Nome);
        Assert.Equal(150m, result.PrecoUnitario);
        Assert.Equal(4, result.QuantidadeEmEstoque);
        await _itemEstoqueRepository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_NotFound_ShouldThrowKeyNotFoundException()
    {
        _itemEstoqueRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ItemEstoque?)null);
        var useCase = new UpdateItemEstoqueUseCase(_itemEstoqueRepository);
        var input = new UpdateItemEstoqueInput("Nome", null, UnidadeMedida.Unidade, 1m);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), input, CancellationToken.None));

        await _itemEstoqueRepository.DidNotReceive().UpdateAsync(Arg.Any<ItemEstoque>(), Arg.Any<CancellationToken>());
    }

    // -------------------------------------------------------------------------
    // Delete
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Delete_SemReserva_ShouldDelete()
    {
        var item = CriarItem(estoque: 2);
        _itemEstoqueRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var useCase = new DeleteItemEstoqueUseCase(_itemEstoqueRepository);

        await useCase.ExecuteAsync(item.Id, CancellationToken.None);

        await _itemEstoqueRepository.Received(1).DeleteAsync(item, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_ComReserva_ShouldThrowInvalidOperationException()
    {
        var item = CriarItem(estoque: 2);
        item.Reservar(1);
        _itemEstoqueRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var useCase = new DeleteItemEstoqueUseCase(_itemEstoqueRepository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(item.Id, CancellationToken.None));

        await _itemEstoqueRepository.DidNotReceive().DeleteAsync(Arg.Any<ItemEstoque>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_NotFound_ShouldThrowKeyNotFoundException()
    {
        _itemEstoqueRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ItemEstoque?)null);
        var useCase = new DeleteItemEstoqueUseCase(_itemEstoqueRepository);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
    }

    // -------------------------------------------------------------------------
    // Entrada de estoque
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Entrada_ShouldIncreaseSaldoAndSave()
    {
        var item = CriarItem(estoque: 2);
        _itemEstoqueRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var useCase = new RegistrarEntradaEstoqueUseCase(_itemEstoqueRepository);

        var result = await useCase.ExecuteAsync(item.Id, new EntradaEstoqueInput(5), CancellationToken.None);

        Assert.Equal(7, result.QuantidadeEmEstoque);
        await _itemEstoqueRepository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Entrada_QuantidadeInvalida_ShouldNotSave()
    {
        var item = CriarItem();
        _itemEstoqueRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var useCase = new RegistrarEntradaEstoqueUseCase(_itemEstoqueRepository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(item.Id, new EntradaEstoqueInput(0), CancellationToken.None));

        await _itemEstoqueRepository.DidNotReceive().UpdateAsync(Arg.Any<ItemEstoque>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Entrada_ItemNotFound_ShouldThrowKeyNotFoundException()
    {
        _itemEstoqueRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ItemEstoque?)null);
        var useCase = new RegistrarEntradaEstoqueUseCase(_itemEstoqueRepository);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), new EntradaEstoqueInput(1), CancellationToken.None));
    }
}
