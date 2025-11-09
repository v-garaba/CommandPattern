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
        var result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Test"));
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
        var result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello"));
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
        var result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hi World"));
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
        var result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(string.Empty));
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

        var result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(string.Empty));

        // Act: Restore from snapshot
        await doc.RestoreSnapshot(snapshot);

        // Assert
        result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Original"));
    }

    [Test]
    public async Task Document_GetText_ReturnsSubstring()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello World");

        // Act
        var textResult = await doc.GetTextAsync(0, 5);

        // Assert
        Assert.That(textResult.IsSuccess, Is.True);
        Assert.That(textResult.Value, Is.EqualTo("Hello"));
    }

    [Test]
    public async Task Document_GetText_WithInvalidPosition_ReturnsEmpty()
    {
        // Arrange
        var doc = new Document();
        await doc.InsertTextAsync(0, "Hello");

        // Act
        var textResult = await doc.GetTextAsync(10, 5);

        // Assert
        Assert.That(textResult.IsSuccess, Is.False);
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
        var result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("He"));
    }

    [Test]
    public async Task Document_Length_ReturnsCorrectValue()
    {
        // Arrange & Act
        var doc = new Document();
        var lengthResult = await doc.GetLengthAsync();
        Assert.That(lengthResult.IsSuccess, Is.True);
        Assert.That(lengthResult.Value, Is.EqualTo(0));

        await doc.InsertTextAsync(0, "Hello");
        lengthResult = await doc.GetLengthAsync();
        Assert.That(lengthResult.IsSuccess, Is.True);
        Assert.That(lengthResult.Value, Is.EqualTo(5));

        await doc.InsertTextAsync(5, " World");
        lengthResult = await doc.GetLengthAsync();
        Assert.That(lengthResult.IsSuccess, Is.True);
        Assert.That(lengthResult.Value, Is.EqualTo(11));
    }
}
