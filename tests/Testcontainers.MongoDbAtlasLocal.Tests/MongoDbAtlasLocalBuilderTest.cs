namespace Testcontainers.MongoDbAtlasLocal;

public sealed class MongoDbAtlasLocalBuilderTest
{
    [Theory]
    [InlineData("mongo", "")]
    [InlineData("", "mongo")]
    public void BuildThrowsArgumentExceptionWhenOnlyOneCredentialIsSet(string username, string password)
    {
        // Given
        var builder = new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile()).WithUsername(username).WithPassword(password);

        // When
        var exception = Assert.Throws<ArgumentException>(() => builder.Build());

        // Then
        Assert.Equal("Credentials", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("seed")]
    [InlineData("seed.json")]
    [InlineData("seed.JS")]
    [InlineData("seed/01-seed.js")]
    [InlineData("seed\\01-seed.js")]
    public void WithInitScriptContentThrowsArgumentExceptionWhenFileNameIsInvalid(string fileName)
    {
        // Given
        var builder = new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile());

        // When
        var exception = Assert.ThrowsAny<ArgumentException>(() => builder.WithInitScriptContent(fileName, string.Empty));

        // Then
        Assert.Equal("fileName", exception.ParamName);
    }

    [Theory]
    [InlineData("01-seed.js")]
    [InlineData("02-seed.sh")]
    public void WithInitScriptContentAcceptsJavaScriptAndShellScripts(string fileName)
    {
        // Given
        var builder = new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile());

        // When
        var exception = Record.Exception(() => builder.WithInitScriptContent(fileName, string.Empty).Build());

        // Then
        Assert.Null(exception);
    }

    [Fact]
    public void WithInitScriptThrowsArgumentExceptionWhenFileExtensionIsNotSupported()
    {
        // Given
        var builder = new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile());

        // When
        var exception = Assert.Throws<ArgumentException>(() => builder.WithInitScript("Seed/movies.json"));

        // Then
        Assert.Equal("scriptFilePath", exception.ParamName);
    }
}