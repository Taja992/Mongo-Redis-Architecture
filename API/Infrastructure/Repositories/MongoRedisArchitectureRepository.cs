using API.Core.Application.Domain.Interfaces;

namespace API.Infrastructure.Repositories;

public class MongoRedisArchitectureRepository : IMongoRedisArchitectureRepository
{
    private readonly AppDbContext _db;

    public MongoRedisArchitectureRepository(AppDbContext db)
    {
        _db = db;
    }
}
