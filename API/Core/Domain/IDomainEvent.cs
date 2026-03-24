namespace API.Core.Domain;

/// <summary>
/// Marker interface for all domain events in this application.
/// Domain events are published after a write completes and consumed by
/// event handlers that sync the read model, cache, and search index.
/// </summary>
public interface IDomainEvent;
