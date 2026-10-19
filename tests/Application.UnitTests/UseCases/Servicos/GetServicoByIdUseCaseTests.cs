using Application.UseCases.Servicos;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;
using System;
using System.Threading;
using Xunit;

namespace Application.UnitTests.UseCases.Servicos
{
    public class GetServicoByIdUseCaseTests
    {
        private readonly IServicoRepository _servicoRepository = Substitute.For<IServicoRepository>();
        private readonly GetServicoByIdUserCase _useCase;

        public GetServicoByIdUseCaseTests()
        {
            _useCase = new GetServicoByIdUserCase(_servicoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidId_ShouldReturnServicoOutput()
        {
            var id = Guid.NewGuid();
            var servico = new Servico("My Service");
            _servicoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(servico);

            var result = await _useCase.ExecuteAsync(id, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("My Service", result.Nome);
        }

        [Fact]
        public async Task ExecuteAsync_WithNotFound_ShouldThrowException()
        {
            var id = Guid.NewGuid();
            _servicoRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Servico?)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(id, CancellationToken.None));
        }
    }
}
