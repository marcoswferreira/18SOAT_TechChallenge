using Application.UseCases.Servicos;
using Application.UseCases.Servicos.Dto;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Application.UnitTests.UseCases.Servicos
{
    public class GetServicoPagedUseCaseTests
    {
        private readonly IServicoRepository _servicoRepository = Substitute.For<IServicoRepository>();
        private readonly GetServicoPagedUseCase _useCase;

        public GetServicoPagedUseCaseTests()
        {
            _useCase = new GetServicoPagedUseCase(_servicoRepository);
        }

        private class TestPaginate : IPaginate<Servico>
        {
            public TestPaginate(IEnumerable<Servico> items, int index = 1, int size = 10)
            {
                Items = items.ToList();
                Index = index;
                Size = size;
                From = 1;
                Count = Items.Count;
                Pages = (int)System.Math.Ceiling(Count / (double)Size);
            }

            public int From { get; }
            public int Index { get; }
            public int Size { get; }
            public int Count { get; }
            public int Pages { get; }
            public IList<Servico> Items { get; }
            public bool HasPrevious => Index - From > 0;
            public bool HasNext => Index - From + 1 < Pages;
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnPagedServicoOutput()
        {
            var services = new[] { new Servico("A"), new Servico("B"), new Servico("C") };
            var page = new TestPaginate(services, index: 1, size: 10);

            _servicoRepository.GetPagedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(page);

            var result = await _useCase.ExecuteAsync(1, 10, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result.Items, i => i.Nome == "A");
            Assert.Contains(result.Items, i => i.Nome == "B");
            Assert.Contains(result.Items, i => i.Nome == "C");
        }
    }
}
