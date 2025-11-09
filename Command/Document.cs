using System.Text;

namespace Command;

public sealed class Document()
{
    private readonly StringBuilder _text = new();

    public string Content => _text.ToString(); // Document State
    public int Length => _text.Length;

    public Document InsertText(int position, string text)
    {
        if (position < 0 || position > _text.Length)
            throw new ArgumentOutOfRangeException(nameof(position));

        var doc = Clone();
        doc._text.Insert(position, text);
        return doc;
    }

    public Document DeleteText(int position, int length)
    {
        if (position < 0 || position >= _text.Length || length <= 0)
            throw new ArgumentOutOfRangeException();

        if (position + length > _text.Length)
            length = _text.Length - position;

        var doc = Clone();
        doc._text.Remove(position, length);
        return doc;
    }

    public Document ReplaceText(int position, int length, string newText)
    {
        var doc = Clone();
        doc = doc.DeleteText(position, length);
        doc = doc.InsertText(position, newText);
        return doc;
    }

    public string GetText(int position, int length)
    {
        if (position < 0 || position >= _text.Length || length <= 0)
            return string.Empty;

        if (position + length > _text.Length)
            length = _text.Length - position;

        return _text.ToString(position, length);
    }

    public Document Clear()
    {
        var document = Clone();
        document._text.Clear();
        return document;
    }

    public override string ToString() => _text.ToString();

    public Document Clone()
    {
        var clone = new Document();
        clone._text.Append(_text.ToString());
        return clone;
    }
}
