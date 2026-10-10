using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Database.DbContexts;

namespace Infrastructure.Database.Repositories;

public class VeiculoRepository(ApplicationDbContext context) : BaseRepository<Veiculo>(context), IVeiculoRepository
{
}