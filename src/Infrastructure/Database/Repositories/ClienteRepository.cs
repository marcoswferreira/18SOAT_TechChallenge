using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;

namespace Infrastructure.Database.Repositories;

public class ClienteRepository(ApplicationDbContext context) : BaseRepository<Cliente>(context), IClienteRepository
{
}