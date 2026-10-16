using Application.UseCases.Servicos;
using Application.UseCases.Servicos.Dto;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;
using System.Threading;
using Xunit;

namespace Application.UnitTests.UseCases.Servicos
{
    public class CreateServicoUseCaseTests
    {
        private readonly IServicoRepository _servicoRepository = Substitute.For<IServicoRepository>();

        private readonly CreateServicoUserCase _useCase;

        public CreateServicoUseCaseTests()
        {
            _useCase = new CreateServicoUserCase(_servicoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ShouldCreateServiceAndReturnOutput()
        {
            var input = new CreateServicoInput("  Test Service  ");
            _servicoRepository.GetByServiceNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns((Servico?)null);

            var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Test Service", result.Nome.Trim());

            await _servicoRepository.Received(1).AddSync(Arg.Is<Servico>(s => s.Nome == "Test Service"), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_WithDuplicateServiceName_ShouldThrowException()
        {
            var input = new CreateServicoInput("Test Service");
            _servicoRepository.GetByServiceNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new Servico("Test Service"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WithNoServiceName_ShouldThrowException()
        {
            var input = new CreateServicoInput("   ");

            await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
        }
    }
}
