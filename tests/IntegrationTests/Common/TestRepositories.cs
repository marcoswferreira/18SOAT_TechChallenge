using System.Collections.Concurrent;
using System.Linq.Expressions;
using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace IntegrationTests.Common;

public class InMemoryBaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ConcurrentDictionary<Guid, T> _items = new();

    protected static Guid GetEntityId(T entity)
    {
        var prop = typeof(T).GetProperty("Id");
        return prop != null ? (Guid)prop.GetValue(entity)! : Guid.Empty;
    }

    public virtual T? GetById(Guid id)
    {
        _items.TryGetValue(id, out var entity);
        return entity;
    }

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _items.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public virtual T Insert(T entity)
    {
        _items[GetEntityId(entity)] = entity;
        return entity;
    }

    public virtual T InsertNotExists(Expression<Func<T, bool>> predicate, T entity)
    {
        var compiled = predicate.Compile();
        if (_items.Values.Any(compiled))
            return entity;

        _items[GetEntityId(entity)] = entity;
        return entity;
    }

    public virtual void Update(T entity)
    {
        _items[GetEntityId(entity)] = entity;
    }

    public virtual void Delete(T entity)
    {
        _items.TryRemove(GetEntityId(entity), out _);
    }

    public virtual void Delete(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
            _items.TryRemove(GetEntityId(entity), out _);
    }

    public virtual bool SaveChanges() => true;

    public virtual Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        _items[GetEntityId(entity)] = entity;
        return Task.FromResult(entity);
    }

    public virtual Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            _items[GetEntityId(entity)] = entity;
        return Task.CompletedTask;
    }

    public virtual Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public virtual Task<IPaginate<T>> GetListAsync(int index = 0, int size = 10, CancellationToken cancellationToken = default)
    {
        IPaginate<T> page = new Paginate<T>(_items.Values, index, size, 0);
        return Task.FromResult(page);
    }

    public virtual IQueryable<T> Query(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool enableTracking = true)
    {
        var query = _items.Values.AsQueryable();
        if (predicate != null)
            query = query.Where(predicate);
        if (orderBy != null)
            query = orderBy(query);
        if (include != null)
            query = include(query);
        return query;
    }

    public void Seed(T entity)
    {
        _items[GetEntityId(entity)] = entity;
    }

    public void Clear()
    {
        _items.Clear();
    }
}

public class InMemoryClienteRepository : InMemoryBaseRepository<Cliente>, IClienteRepository
{
    public Task<Cliente?> GetByIdWithVeiculosAsync(Guid id, CancellationToken cancellationToken = default)
        => GetByIdAsync(id, cancellationToken);

    public Task<Cliente?> GetByDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        var docDigits = new string(documento.Where(char.IsDigit).ToArray());
        var cliente = _items.Values.FirstOrDefault(c =>
            c.Documento.Valor == docDigits || c.Documento.Formatado == documento);
        return Task.FromResult(cliente);
    }

    public Task<bool> ExistsByDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        var docDigits = new string(documento.Where(char.IsDigit).ToArray());
        var exists = _items.Values.Any(c =>
            c.Documento.Valor == docDigits || c.Documento.Formatado == documento);
        return Task.FromResult(exists);
    }

    public Task<IPaginate<Cliente>> GetPagedAsync(int index, int size, CancellationToken cancellationToken = default)
        => GetListAsync(index, size, cancellationToken);
}

public class InMemoryVeiculoRepository : InMemoryBaseRepository<Veiculo>, IVeiculoRepository
{
    public Task<Veiculo?> GetByPlacaAsync(string placa, CancellationToken cancellationToken = default)
    {
        var veiculo = _items.Values.FirstOrDefault(v =>
            v.Placa.Valor.Equals(placa, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(veiculo);
    }

    public Task<bool> ExistsByPlacaAsync(string placa, CancellationToken cancellationToken = default)
    {
        var exists = _items.Values.Any(v =>
            v.Placa.Valor.Equals(placa, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<IEnumerable<Veiculo>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var veiculos = _items.Values.Where(v => v.ClienteId == clienteId);
        return Task.FromResult<IEnumerable<Veiculo>>(veiculos);
    }

    public Task<IPaginate<Veiculo>> GetPagedAsync(int index, int size, CancellationToken cancellationToken = default)
        => GetListAsync(index, size, cancellationToken);
}

public class InMemoryItemEstoqueRepository : InMemoryBaseRepository<ItemEstoque>, IItemEstoqueRepository
{
    public Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        var exists = _items.Values.Any(i => i.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<IPaginate<ItemEstoque>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
        => GetListAsync(index, size, cancellationToken);

    public Task AddAsync(ItemEstoque item, CancellationToken cancellationToken)
        => InsertAsync(item, cancellationToken);

    Task IItemEstoqueRepository.UpdateAsync(ItemEstoque item, CancellationToken cancellationToken)
    {
        _items[item.Id] = item;
        return Task.CompletedTask;
    }

    Task IItemEstoqueRepository.DeleteAsync(ItemEstoque item, CancellationToken cancellationToken)
    {
        _items.TryRemove(item.Id, out _);
        return Task.CompletedTask;
    }
}

public class InMemoryServicoRepository : InMemoryBaseRepository<Servico>, IServicoRepository
{
    public Task<Servico?> GetByServiceNameAsync(string serviceName, CancellationToken cancellationToken)
    {
        var servico = _items.Values.FirstOrDefault(s =>
            s.Nome.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(servico);
    }

    public Task<IPaginate<Servico>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
        => GetListAsync(index, size, cancellationToken);

    public Task AddSync(Servico service, CancellationToken cancellationToken)
        => InsertAsync(service, cancellationToken);

    public Task UpdateSync(Servico service, CancellationToken cancellationToken)
    {
        _items[service.Id] = service;
        return Task.CompletedTask;
    }

    public Task DeleteSync(Servico service, CancellationToken cancellationToken)
    {
        _items.TryRemove(service.Id, out _);
        return Task.CompletedTask;
    }
}

public class InMemoryUserRepository : InMemoryBaseRepository<User>, IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = _items.Values.FirstOrDefault(u => u.Email == normalized);
        return Task.FromResult(user);
    }

    public Task<User?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        var user = _items.Values.FirstOrDefault(u => u.RefreshTokenHash == refreshTokenHash);
        return Task.FromResult(user);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var exists = _items.Values.Any(u => u.Email == normalized);
        return Task.FromResult(exists);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
        => InsertAsync(user, cancellationToken);

    Task IUserRepository.UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _items[user.Id] = user;
        return Task.CompletedTask;
    }

    Task IUserRepository.DeleteAsync(User user, CancellationToken cancellationToken)
    {
        _items.TryRemove(user.Id, out _);
        return Task.CompletedTask;
    }

    public Task<IPaginate<User>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
        => GetListAsync(index, size, cancellationToken);
}

public class InMemoryOrdemServicoRepository : InMemoryBaseRepository<OrdemServico>, IOrdemServicoRepository
{
    private readonly Func<Guid, Cliente?>? _clienteLookup;

    public InMemoryOrdemServicoRepository(Func<Guid, Cliente?>? clienteLookup = null)
    {
        _clienteLookup = clienteLookup;
    }

    public Task<OrdemServico?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => GetByIdAsync(id, cancellationToken);

    public Task<IEnumerable<OrdemServico>> GetByDocumentoClienteAsync(Documento documento, CancellationToken cancellationToken = default)
    {
        var ordens = _items.Values.Where(os =>
        {
            if (os.Cliente?.Documento == documento)
                return true;
            if (_clienteLookup != null)
            {
                var cli = _clienteLookup(os.ClienteId);
                return cli?.Documento == documento;
            }
            return false;
        }).ToList();

        return Task.FromResult<IEnumerable<OrdemServico>>(ordens);
    }

    public Task<IPaginate<OrdemServico>> GetPagedAsync(int index, int size, CancellationToken cancellationToken = default)
        => GetListAsync(index, size, cancellationToken);
}
