using System.Text;
using Command.Memento;

namespace Command;

public sealed class Document()
{
    private readonly StringBuilder _text = new();

    public string Content => _text.ToString(); // Document State
    public int Length => _text.Length;

    public void InsertText(int position, string text)
    {
        if (position < 0 || position > _text.Length)
            throw new ArgumentOutOfRangeException(nameof(position));

        _text.Insert(position, text);
    }

    public void DeleteText(int position, int length)
    {
        if (position < 0 || position >= _text.Length || length <= 0)
            throw new ArgumentOutOfRangeException();

        if (position + length > _text.Length)
            length = _text.Length - position;

        _text.Remove(position, length);
    }

    public void ReplaceText(int position, int length, string newText)
    {
        DeleteText(position, length);
        InsertText(position, newText);
    }

    public string GetText(int position, int length)
    {
        if (position < 0 || position >= _text.Length || length <= 0)
            return string.Empty;

        if (position + length > _text.Length)
            length = _text.Length - position;

        return _text.ToString(position, length);
    }

    public void Clear()
    {
        _text.Clear();
    }

    public override string ToString() => _text.ToString();

    public ISnapshot CreateSnapshot()
    {
        return new DocumentState(Content);
    }

    public void RestoreSnapshot(ISnapshot snapshot)
    {
        if (snapshot is not DocumentState docSnapshot)
            throw new ArgumentException("Invalid snapshot type", nameof(snapshot));

        _text.Clear();
        _text.Append(docSnapshot.Content);
    }

    private sealed class DocumentState(string content) : ISnapshot
    {
        public string Content { get; } = content;
    }
}
