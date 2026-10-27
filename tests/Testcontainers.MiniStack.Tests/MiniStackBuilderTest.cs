namespace Testcontainers.MiniStack;

public sealed class MiniStackBuilderTest
{
    [Theory]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [InlineData("")]
    [InlineData(" ")]
    public void ThrowsArgumentExceptionWhenRegionIsEmpty(string region)
    {
        var miniStackBuilder = new MiniStackBuilder(TestSession.GetImageFromDockerfile()).WithRegion(region);
        Assert.Throws<ArgumentException>(miniStackBuilder.Build);
    }

    [Theory]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [InlineData("", MiniStackBuilder.SecretKey)]
    [InlineData(MiniStackBuilder.AccessKey, "")]
    public void ThrowsArgumentExceptionWhenCredentialsAreIncomplete(string accessKey, string secretKey)
    {
        var miniStackBuilder = new MiniStackBuilder(TestSession.GetImageFromDockerfile()).WithCredentials(accessKey, secretKey);
        Assert.Throws<ArgumentException>(miniStackBuilder.Build);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void AccessorsReturnDefaultsWhenNotConfigured()
    {
        var miniStackContainer = new MiniStackBuilder(TestSession.GetImageFromDockerfile()).Build();
        Assert.Equal(MiniStackBuilder.Region, miniStackContainer.GetRegion());
        Assert.Equal(MiniStackBuilder.AccessKey, miniStackContainer.GetAccessKey());
        Assert.Equal(MiniStackBuilder.SecretKey, miniStackContainer.GetSecretKey());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ThrowsArgumentExceptionWhenServicesAreEmpty()
    {
        var miniStackBuilder = new MiniStackBuilder(TestSession.GetImageFromDockerfile()).WithServices();
        Assert.Throws<ArgumentException>(miniStackBuilder.Build);
    }

    [Theory]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [ClassData(typeof(InvalidScriptConfigurations))]
    public void ThrowsArgumentExceptionWhenScriptIsInvalid(MiniStackBuilder miniStackBuilder)
    {
        Assert.Throws<ArgumentException>(miniStackBuilder.Build);
    }

    private sealed class InvalidScriptConfigurations : TheoryData<MiniStackBuilder>
    {
        public InvalidScriptConfigurations()
        {
            var miniStackBuilder = new MiniStackBuilder(TestSession.GetImageFromDockerfile());
            Add(miniStackBuilder.WithInitScript("01-boot.txt"));
            Add(miniStackBuilder.WithInitScriptContent("scripts/01-boot.sh", string.Empty));
            Add(miniStackBuilder.WithInitScriptContent("01-boot.sh", null));
            Add(miniStackBuilder.WithReadyScript("01-seed.js"));
            Add(miniStackBuilder.WithReadyScriptContent("scripts\\01-seed.sh", string.Empty));
            Add(miniStackBuilder.WithReadyScriptContent(string.Empty, string.Empty));
            Add(miniStackBuilder.WithReadyScriptContent("01-seed.py", null));
        }
    }
}
