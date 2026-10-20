using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Contrato genérico de repositório (Porta no Domain).
/// Desacoplado de ORM específico: não utiliza tipos do Entity Framework Core.
/// </summary>
/// <typeparam name="T">O tipo da entidade gerenciada.</typeparam>
public interface IBaseRepository<T> where T : class
{
    IQueryable<T> Query(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool enableTracking = true);

    T Insert(T entity);
    T InsertNotExists(Expression<Func<T, bool>> predicate, T entity);
    void Update(T entity);
    void Delete(T entity);
    void Delete(IEnumerable<T> entities);
    bool SaveChanges();
    Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);
    Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}

