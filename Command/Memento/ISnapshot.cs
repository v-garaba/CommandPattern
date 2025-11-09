namespace Command.Memento;

/// <summary>
/// Enhanced snapshot interface following Microsoft's recommended pattern.
/// Includes metadata and versioning support.
/// </summary>
public interface ISnapshot
{
    /// <summary>
    /// Gets the unique identifier for this snapshot.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the timestamp when the snapshot was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the version of the snapshot format.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets optional metadata associated with the snapshot.
    /// </summary>
    IReadOnlyDictionary<string, object>? Metadata { get; }
}