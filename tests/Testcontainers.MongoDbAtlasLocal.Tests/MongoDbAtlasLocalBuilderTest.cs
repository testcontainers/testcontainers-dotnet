namespace Testcontainers.MongoDbAtlasLocal;

public sealed class MongoDbAtlasLocalBuilderTest
{
    [Theory]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [ClassData(typeof(MongoDbAtlasLocalBuilderConfigurations))]
    public void ThrowsArgumentExceptionWhenInitScriptIsInvalid(MongoDbAtlasLocalBuilder mongoDbAtlasLocalBuilder)
    {
        Assert.Throws<ArgumentException>(mongoDbAtlasLocalBuilder.Build);
    }

    [Theory]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [InlineData("", MongoDbAtlasLocalBuilder.DefaultPassword)]
    [InlineData(MongoDbAtlasLocalBuilder.DefaultUsername, "")]
    public void ThrowsArgumentExceptionWhenCredentialsAreIncomplete(string username, string password)
    {
        var mongoDbAtlasLocalBuilder = new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile()).WithUsername(username).WithPassword(password);
        Assert.Throws<ArgumentException>(mongoDbAtlasLocalBuilder.Build);
    }

    private sealed class MongoDbAtlasLocalBuilderConfigurations : TheoryData<MongoDbAtlasLocalBuilder>
    {
        public MongoDbAtlasLocalBuilderConfigurations()
        {
            var mongoDbAtlasLocalBuilder = new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile());
            Add(mongoDbAtlasLocalBuilder.WithInitScript("01-movies.txt"));
            Add(mongoDbAtlasLocalBuilder.WithInitScriptContent("01-movies.txt", string.Empty));
            Add(mongoDbAtlasLocalBuilder.WithInitScriptContent("01-movies.JS", string.Empty));
            Add(mongoDbAtlasLocalBuilder.WithInitScriptContent("scripts/01-movies.js", string.Empty));
            Add(mongoDbAtlasLocalBuilder.WithInitScriptContent("scripts\\01-movies.js", string.Empty));
            Add(mongoDbAtlasLocalBuilder.WithInitScriptContent(string.Empty, string.Empty));
            Add(mongoDbAtlasLocalBuilder.WithInitScriptContent("01-movies.js", null));
        }
    }
}