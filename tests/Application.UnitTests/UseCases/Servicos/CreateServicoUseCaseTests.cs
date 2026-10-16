using Application.UseCases.Servicos;
using Application.UseCases.Servicos.Dto;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Apolication.UnitTests.UseCases.Servicos
{
    public class CreateServicoUseCaseTests
    {
        private readonly IServicoRepository _servicoRepository = Substitute.For<IServicoRepository>();

        private readonly CreateServicoUserCase _useCase;

        public CreateServicoUseCaseTests()
        {
            _useCase = new CreateServicoUserCase(_servicoRepository);
        }

        // -------------------------------------------------------------------------
        // Happy paths
        // -------------------------------------------------------------------------
        [Fact]
        public async Task ExecuteAsync_WithValidInput_ShouldCreateServiceAndReturnOutput()
        {
            var input = new CreateServicoInput("Test Service");
            var result = await _useCase.ExecuteAsync(input, CancellationToken.None);
        }


        // -------------------------------------------------------------------------
        // Validation errors
        // -------------------------------------------------------------------------
        [Fact]
        public async Task ExecuteAsync_WithDuplicateServiceName_ShouldThrowException()
        {
            var input = new CreateServicoInput("Test Service");
            var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        }

        public async Task ExecuteAsync_WithNoServiceName_ShouldThrowException()
        {
            var input = new CreateServicoInput("");
            var result = await _useCase.ExecuteAsync(input, CancellationToken.None);
        }


    }
}