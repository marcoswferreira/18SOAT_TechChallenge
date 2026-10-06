using Domain.Common.Paginate;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshTokenHash, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbSet
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await InsertAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken)
    {
        Delete(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IPaginate<User>> GetPagedAsync(int index, int size, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .ToPaginateAsync(index, size, 0, cancellationToken);
    }
}