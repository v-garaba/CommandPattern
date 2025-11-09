using System.Text;
using Command.Memento;

namespace Command;

public sealed class Document : ISnapshotRestorable
{
    private readonly StringBuilder _text = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<bool> InsertTextAsync(
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

    public async Task<bool> DeleteTextAsync(
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

    public async Task<bool> ReplaceTextAsync(
        int position,
        int length,
        string newText,
        CancellationToken cancellationToken = default
    )
    {
        bool result = false;
        await _lock.WaitAsync(cancellationToken);
        try
        {
            result = InternalDeleteText(position, length);

            if (result == false)
                return false;

            result = InternalInsertText(position, newText);
        }
        finally
        {
            _lock.Release();
        }

        return result;
    }

    public async Task<string> GetTextAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _text.ToString();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<int> GetLengthAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _text.Length;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<string> GetTextAsync(int position, int length, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (position < 0 || position >= _text.Length || length <= 0)
                return string.Empty;

            if (position + length > _text.Length)
                length = _text.Length - position;

            return _text.ToString(position, length);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            _text.Clear();
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool InternalInsertText(int position, string text)
    {
        if (position < 0 || position > _text.Length)
            return false;

        _text.Insert(position, text);
        return true;
    }

    private bool InternalDeleteText(int position, int length)
    {
        if (position < 0 || position >= _text.Length || length <= 0)
            return false;

        if (position + length > _text.Length)
            length = _text.Length - position;

        _text.Remove(position, length);
        return true;
    }

    public override string ToString() => _text.ToString();

    #region ISnapshotRestorable Members
    public ISnapshot CreateSnapshot()
    {
        return new DocumentState(_text.ToString());
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

    private sealed class DocumentState(string content) : ISnapshot
    {
        public string Content { get; } = content;
    }
    #endregion
}
