using API.Core.Application.Services;
using API.Core.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace UnitTests.Services;

public class MongoRedisArchitectureServiceTests
{
    private readonly IMongoRedisArchitectureRepository _repository =
        Substitute.For<IMongoRedisArchitectureRepository>();
    private readonly MongoRedisArchitectureService _sut;

    public MongoRedisArchitectureServiceTests()
    {
        _sut = new MongoRedisArchitectureService(_repository);
    }

    // TODO: add tests here
}
