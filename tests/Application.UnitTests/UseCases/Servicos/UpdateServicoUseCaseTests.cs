using Application.UseCases.Servicos;
using Application.UseCases.Servicos.Dto;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Services
{
    public class UpdateServicoUseCaseTests
    {
        private readonly IServicoRepository _servicoRepository = Substitute.For<IServicoRepository>();

        private readonly UpdateServicoUserCase _useCase;

        public UpdateServicoUseCaseTests()
        {
            _useCase = new UpdateServicoUserCase(_servicoRepository);
        }

        public async Task ExecuteAsync_WithNoIdExists_ShouldThrowException()
        {
            var input = new UpdateServicoInput(Guid.NewGuid(), "Test Service");


        }
    }
}
