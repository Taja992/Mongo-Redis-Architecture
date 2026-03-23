using API.Core.Application.Interfaces;
using API.Core.Domain.Interfaces;

namespace API.Core.Application.Services;

public class MongoRedisArchitectureService : IMongoRedisArchitectureService
{
    private readonly IMongoRedisArchitectureRepository _mongoredisarchitectureRepository;

    public MongoRedisArchitectureService(
        IMongoRedisArchitectureRepository mongoredisarchitectureRepository
    )
    {
        _mongoredisarchitectureRepository = mongoredisarchitectureRepository;
    }
}
