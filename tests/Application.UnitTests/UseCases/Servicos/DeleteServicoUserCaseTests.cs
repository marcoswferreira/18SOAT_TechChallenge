using Application.UseCases.Servicos;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;
using System;
using System.Threading;
using Xunit;

namespace Application.UnitTests.UseCases.Servicos
{
    public class DeleteServicoUserCaseTests
    {
        private readonly IServicoRepository _servicoRepository = Substitute.For<IServicoRepository>();
        private readonly DeleteServicoUserCase _useCase;

        public DeleteServicoUserCaseTests()
        {
            _useCase = new DeleteServicoUserCase(_servicoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_WithNotFound_ShouldThrowException()
        {
            var id = Guid.NewGuid();
            _servicoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Servico?)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WithValidId_ShouldCallDelete()
        {
            var id = Guid.NewGuid();
            var servico = new Servico("To Delete");
            _servicoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(servico);

            await _useCase.ExecuteAsync(id, CancellationToken.None);

            await _servicoRepository.Received(1).DeleteSync(Arg.Is<Servico>(s => s.Nome == "To Delete"), Arg.Any<CancellationToken>());
        }
    }
}
