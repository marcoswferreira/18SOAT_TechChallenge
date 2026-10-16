using Application.UseCases.Services;
using Application.UseCases.Services.Dto;
using Domain.Interfaces.Repositories;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UnitTests.UseCases.Services
{
    public class UpdateServiceUseCaseTests
    {
        private readonly IServiceRepository _serviceRepository = Substitute.For<IServiceRepository>();

        private readonly UpdateServiceUserCase _useCase;

        public UpdateServiceUseCaseTests()
        {
            _useCase = new UpdateServiceUserCase(_serviceRepository);
        }

        public async Task ExecuteAsync_WithNoIdExists_ShouldThrowException()
        {
            var input = new UpdateServiceInput(Guid.NewGuid(), "Test Service");


        }
    }
}
