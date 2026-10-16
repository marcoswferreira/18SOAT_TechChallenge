using Application.UseCases.Servicos;
using Application.UseCases.Servicos.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;
using System;
using System.Threading;
using Xunit;

namespace Application.UnitTests.UseCases.Servicos
{
    public class UpdateServicoUseCaseTests
    {
        private readonly IServicoRepository _servicoRepository = Substitute.For<IServicoRepository>();

        private readonly UpdateServicoUserCase _useCase;

        public UpdateServicoUseCaseTests()
        {
            _useCase = new UpdateServicoUserCase(_servicoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_WithNoIdExists_ShouldThrowException()
        {
            var id = Guid.NewGuid();
            var input = new UpdateServicoInput(id, "Test Service");
            _servicoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Servico?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(id, input, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidName_ShouldThrowException()
        {
            var id = Guid.NewGuid();
            var servico = new Servico("Old Name");
            _servicoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(servico);

            var input = new UpdateServicoInput(id, "");

            await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(id, input, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ShouldUpdateAndReturnOutput()
        {
            var id = Guid.NewGuid();
            var servico = new Servico("Old Name");
            _servicoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(servico);

            var input = new UpdateServicoInput(id, "  New Name  "   );

            var result = await _useCase.ExecuteAsync(id, input, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("New Name", result.Nome);

            await _servicoRepository.Received(1).UpdateSync(Arg.Is<Servico>(s => s.Nome == "New Name"), Arg.Any<CancellationToken>());
        }
    }
}
