using System.Text.Json;
using API.Core.Application.Events;
using API.Core.Domain.Interfaces;
using API.Core.Domain.WriteModels;
using MediatR;

namespace API.Core.Application.Commands.Posts;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand>
{
    private readonly IPostWriteRepository _writeRepository;
    private readonly IPublisher _publisher;

    public UpdatePostCommandHandler(IPostWriteRepository writeRepository, IPublisher publisher)
    {
        _writeRepository = writeRepository;
        _publisher = publisher;
    }

    public async Task Handle(UpdatePostCommand command, CancellationToken cancellationToken)
    {
        PostWriteModel? post = await _writeRepository.GetByIdAsync(
            command.PostId,
            cancellationToken
        );
        if (post is null)
            return;

        // Resolve the final tag list before mutating the model so the event carries
        // the complete state regardless of which fields the caller provided.
        List<string> resolvedTags =
            command.Tags ?? JsonSerializer.Deserialize<List<string>>(post.Tags) ?? [];

        if (command.Title is not null)
            post.Title = command.Title;
        if (command.Body is not null)
            post.Body = command.Body;
        if (command.Tags is not null)
            post.Tags = JsonSerializer.Serialize(command.Tags);
        post.UpdatedAt = DateTime.UtcNow;

        await _writeRepository.UpdateAsync(post, cancellationToken);

        await _publisher.Publish(
            new PostUpdatedEvent(
                post.Id,
                post.BlogId,
                post.Title,
                post.Body,
                resolvedTags,
                post.UpdatedAt
            ),
            cancellationToken
        );
    }
}
