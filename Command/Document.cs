using System.Text;
using Command.Common;
using Command.Memento;

namespace Command;

public sealed class Document : ISnapshotRestorable
{
    private readonly StringBuilder _text = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<Result> InsertTextAsync(
        int position,
        string text,
        CancellationToken cancellationToken = default
    )
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return InternalInsertText(position, text);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Result> DeleteTextAsync(
        int position,
        int length,
        CancellationToken cancellationToken = default
    )
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return InternalDeleteText(position, length);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Result> ReplaceTextAsync(
        int position,
        int length,
        string newText,
        CancellationToken cancellationToken = default
    )
    {
        Result result = Result.Success();
        await _lock.WaitAsync(cancellationToken);
        try
        {
            result = InternalDeleteText(position, length);

            if (!result.IsSuccess)
                return result;

            result = InternalInsertText(position, newText);
        }
        finally
        {
            _lock.Release();
        }

        return result;
    }

    public async Task<Result<string>> GetTextAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return Result<string>.Success(_text.ToString());
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Result<int>> GetLengthAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return Result<int>.Success(_text.Length);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Result<string>> GetTextAsync(int position, int length, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (position < 0 || position >= _text.Length || length <= 0)
                return Result<string>.Failure("Invalid position or length");

            if (position + length > _text.Length)
                length = _text.Length - position;

            return Result<string>.Success(_text.ToString(position, length));
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Result> ClearAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            _text.Clear();
            return Result.Success();
        }
        finally
        {
            _lock.Release();
        }
    }

    private Result InternalInsertText(int position, string text)
    {
        if (position < 0)
            return "Invalid position for insertion. The position cannot be negative.";

        if (position > _text.Length)
            return "Invalid position for insertion. The position exceeds document length.";

        _text.Insert(position, text);
        return Result.Success();
    }

    private Result InternalDeleteText(int position, int length)
    {
        if (position < 0)
            return "Invalid position or length for deletion. The position cannot be negative.";

        if (position >= _text.Length )
            return "Invalid position or length for deletion. The position exceeds document length.";

        if (length <= 0)
            return "Invalid position or length for deletion. The length must be positive.";

        if (position + length > _text.Length)
            length = _text.Length - position;

        _text.Remove(position, length);
        return Result.Success();
    }

    public override string ToString() => _text.ToString();

    #region ISnapshotRestorable Implementation
    public ISnapshot CreateSnapshot()
    {
        return new DocumentState(
            _text.ToString(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            "1.0",
            new Dictionary<string, object>
            {
                ["SnapshotType"] = "DocumentState",
            }
        );
    }

    public async Task RestoreSnapshot(ISnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (snapshot is not DocumentState docSnapshot)
                throw new ArgumentException("Invalid snapshot type", nameof(snapshot));

            _text.Clear();
            _text.Append(docSnapshot.Content);
        }
        finally
        {
            _lock.Release();
        }
    }

    private sealed record DocumentState(
        string Content,
        Guid Id,
        DateTimeOffset CreatedAt,
        string Version,
        IReadOnlyDictionary<string, object>? Metadata
    ) : ISnapshot;
    #endregion
}
