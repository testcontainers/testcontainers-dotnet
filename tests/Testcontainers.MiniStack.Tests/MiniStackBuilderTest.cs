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
}
