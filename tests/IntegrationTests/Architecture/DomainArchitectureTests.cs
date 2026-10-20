using Domain.Entities;
using Xunit;

namespace IntegrationTests.Architecture;

/// <summary>
/// Garante que o projeto Domain seja 100% puro e não possua qualquer dependência
/// do Entity Framework Core ou de outros pacotes de infraestrutura/persistência.
/// </summary>
public class DomainArchitectureTests
{
    [Fact]
    public void Domain_NaoDeveReferenciarEntityFrameworkCore()
    {
        var domainAssembly = typeof(User).Assembly;

        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        var efCoreReferences = referencedAssemblies
            .Where(a => a.Name is not null && a.Name.Contains("EntityFrameworkCore", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Empty(efCoreReferences);
    }
}

