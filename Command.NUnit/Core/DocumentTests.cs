namespace Command.NUnit;

[TestFixture]
public class DocumentTests
{
    [Test]
    public void Document_InsertText_ReturnsNewInstance()
    {
        // Arrange
        var doc = new Document();

        // Act
        doc.InsertText(0, "Test");

        // Assert
        Assert.That(doc.Content, Is.EqualTo("Test"));
    }

    [Test]
    public void Document_DeleteText_ReturnsNewInstance()
    {
        // Arrange
        var doc = new Document();
        doc.InsertText(0, "Hello World");

        // Act
        doc.DeleteText(5, 6);

        // Assert
        Assert.That(doc.Content, Is.EqualTo("Hello"));
    }

    [Test]
    public void Document_ReplaceText_ReturnsNewInstance()
    {
        // Arrange
        var doc = new Document();
        doc.InsertText(0, "Hello World");

        // Act
        doc.ReplaceText(0, 5, "Hi");

        // Assert
        Assert.That(doc.Content, Is.EqualTo("Hi World"));
    }

    [Test]
    public void Document_Clear_EmptiesContent()
    {
        // Arrange
        var doc = new Document();
        doc.InsertText(0, "Hello World");

        // Act
        doc.Clear();

        // Assert
        Assert.That(doc.Content, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Document_CreateSnapshot_CreatesDeepCopy()
    {
        // Arrange
        var doc = new Document();
        doc.InsertText(0, "Original");

        // Act & Assert: Create snapshot then clear original
        var snapshot = doc.CreateSnapshot();

        doc.Clear();

        Assert.That(doc.Content, Is.EqualTo(string.Empty));

        // Act: Restore from snapshot
        doc.RestoreSnapshot(snapshot);

        // Assert
        Assert.That(doc.Content, Is.EqualTo("Original"));
    }

    [Test]
    public void Document_GetText_ReturnsSubstring()
    {
        // Arrange
        var doc = new Document();
        doc.InsertText(0, "Hello World");

        // Act
        var text = doc.GetText(0, 5);

        // Assert
        Assert.That(text, Is.EqualTo("Hello"));
    }

    [Test]
    public void Document_GetText_WithInvalidPosition_ReturnsEmpty()
    {
        // Arrange
        var doc = new Document();
        doc.InsertText(0, "Hello");

        // Act
        var text = doc.GetText(10, 5);

        // Assert
        Assert.That(text, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Document_InsertText_AtInvalidPosition_ThrowsException()
    {
        // Arrange
        var doc = new Document();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => doc.InsertText(-1, "Test"));
        Assert.Throws<ArgumentOutOfRangeException>(() => doc.InsertText(10, "Test"));
    }

    [Test]
    public void Document_DeleteText_BeyondLength_DeletesToEnd()
    {
        // Arrange
        var doc = new Document();
        doc.InsertText(0, "Hello");

        // Act
        doc.DeleteText(2, 100);

        // Assert
        Assert.That(doc.Content, Is.EqualTo("He"));
    }

    [Test]
    public void Document_Length_ReturnsCorrectValue()
    {
        // Arrange & Act
        var doc = new Document();
        Assert.That(doc.Length, Is.EqualTo(0));

        doc.InsertText(0, "Hello");
        Assert.That(doc.Length, Is.EqualTo(5));

        doc.InsertText(5, " World");
        Assert.That(doc.Length, Is.EqualTo(11));
    }
}
