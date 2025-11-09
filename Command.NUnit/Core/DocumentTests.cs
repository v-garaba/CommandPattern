namespace Command.NUnit;

[TestFixture]
public class DocumentTests
{
    [Test]
    public async Task Document_InsertText_ReturnsNewInstance()
    {
        // Arrange
        var doc = new Document();

        // Act
        await doc.InsertTextAsync(0, "Test");

        // Assert
        string content = await doc.GetTextAsync();
        Assert.That(content, Is.EqualTo("Test"));
    }

    [Test]
    public async Task Document_DeleteText_ReturnsNewInstance()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello World");

        // Act
        await doc.DeleteTextAsync(5, 6);

        // Assert
        string content = await doc.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello"));
    }

    [Test]
    public async Task Document_ReplaceText_ReturnsNewInstance()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello World");

        // Act
        await doc.ReplaceTextAsync(0, 5, "Hi");

        // Assert
        string content = await doc.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hi World"));
    }

    [Test]
    public async Task Document_Clear_EmptiesContent()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello World");

        // Act
        await doc.ClearAsync();

        // Assert
        string content = await doc.GetTextAsync();
        Assert.That(content, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task Document_CreateSnapshot_CreatesDeepCopy()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Original");

        // Act & Assert: Create snapshot then clear original
        var snapshot = doc.CreateSnapshot();

        await doc.ClearAsync();

        string content = await doc.GetTextAsync();
        Assert.That(content, Is.EqualTo(string.Empty));

        // Act: Restore from snapshot
        await doc.RestoreSnapshot(snapshot);

        // Assert
        content = await doc.GetTextAsync();
        Assert.That(content, Is.EqualTo("Original"));
    }

    [Test]
    public async Task Document_GetText_ReturnsSubstring()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello World");

        // Act
        var text = await doc.GetTextAsync(0, 5);

        // Assert
        Assert.That(text, Is.EqualTo("Hello"));
    }

    [Test]
    public async Task Document_GetText_WithInvalidPosition_ReturnsEmpty()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello");

        // Act
        var text = await doc.GetTextAsync(10, 5);

        // Assert
        Assert.That(text, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task Document_DeleteText_BeyondLength_DeletesToEnd()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello");

        // Act
        await doc.DeleteTextAsync(2, 100);

        // Assert
        string content = await doc.GetTextAsync();
        Assert.That(content, Is.EqualTo("He"));
    }

    [Test]
    public async Task Document_Length_ReturnsCorrectValue()
    {
        // Arrange & Act
        var doc = new Document();
        int length = await doc.GetLengthAsync();
        Assert.That(length, Is.EqualTo(0));

        await doc.InsertTextAsync(0, "Hello");
        length = await doc.GetLengthAsync();
        Assert.That(length, Is.EqualTo(5));

        await doc.InsertTextAsync(5, " World");
        length = await doc.GetLengthAsync();
        Assert.That(length, Is.EqualTo(11));
    }
}
