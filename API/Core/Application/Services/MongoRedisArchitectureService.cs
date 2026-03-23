using API.Core.Application.Domain.Interfaces;
using API.Core.Application.Interfaces;

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
