using API.Core.Domain.Exceptions;

namespace API.Core.Domain.Exceptions.Validation;

public class MongoRedisArchitectureException : AppException
{
    public MongoRedisArchitectureException()
        : base("MongoRedisArchitecture exception occurred.", "TEMPLATE_EXCEPTION", 400) { }

    public MongoRedisArchitectureException(string message)
        : base(message, "TEMPLATE_EXCEPTION", 400) { }
}
