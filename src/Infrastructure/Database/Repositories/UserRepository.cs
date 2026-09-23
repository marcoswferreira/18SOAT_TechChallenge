using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class UserRepository(DbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshTokenHash, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}