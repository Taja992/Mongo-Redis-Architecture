using System.Text.Json;
using API.Core.Application.Events;
using API.Core.Domain.Interfaces;
using API.Core.Domain.WriteModels;
using MediatR;
using MongoDB.Bson;

namespace API.Core.Application.Commands.Posts;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, string>
{
    private readonly IPostWriteRepository _writeRepository;
    private readonly IPublisher _publisher;

    public CreatePostCommandHandler(IPostWriteRepository writeRepository, IPublisher publisher)
    {
        _writeRepository = writeRepository;
        _publisher = publisher;
    }

    public async Task<string> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        // Generate the shared key before writing to either store.
        // MongoDB ObjectIds are created client-side — no coordination needed.
        PostWriteModel post = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            BlogId = command.BlogId,
            AuthorId = command.AuthorId,
            Title = command.Title,
            Body = command.Body,
            Tags = JsonSerializer.Serialize(command.Tags),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _writeRepository.CreateAsync(post, cancellationToken);

        // Publish triggers PostCreatedEventHandler which syncs MongoDB synchronously.
        // By the time this line returns, the read model is already consistent.
        await _publisher.Publish(
            new PostCreatedEvent(
                post.Id,
                post.BlogId,
                post.AuthorId,
                post.Title,
                post.Body,
                command.Tags,
                post.CreatedAt
            ),
            cancellationToken
        );

        return post.Id;
    }
}
