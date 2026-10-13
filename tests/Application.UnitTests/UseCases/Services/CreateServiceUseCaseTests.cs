using Application.UseCases.Services;
using Application.UseCases.Services.Dto;
using Domain.Interfaces.Repositories;
using NSubstitute;

namespace Application.UnitTests.UseCases.Services
{
    public class CreateServiceUseCaseTests
    {
        private readonly IServiceRepository _serviceRepository = Substitute.For<IServiceRepository>();

        private readonly CreateServicesUseCase _useCase;

        public CreateServiceUseCaseTests()
        {
            _useCase = new CreateServicesUseCase(_serviceRepository);
        }

        // -------------------------------------------------------------------------
        // Happy paths
        // -------------------------------------------------------------------------
        [Fact]
        public async Task ExecuteAsync_WithValidInput_ShouldCreateServiceAndReturnOutput()
        {
            var input = new CreateServiceInput("Test Service");
            var result = await _useCase.ExecuteAsync(input, CancellationToken.None);
        }


        // -------------------------------------------------------------------------
        // Validation errors
        // -------------------------------------------------------------------------
        [Fact]
        public async Task ExecuteAsync_WithDuplicateServiceName_ShouldThrowException()
        {
            var input = new CreateServiceInput("Test Service");
            var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        }

        public async Task ExecuteAsync_WithNoServiceName_ShouldThrowException()
        {
            var input = new CreateServiceInput("");
            var result = await _useCase.ExecuteAsync(input, CancellationToken.None);
        }

        
    }
