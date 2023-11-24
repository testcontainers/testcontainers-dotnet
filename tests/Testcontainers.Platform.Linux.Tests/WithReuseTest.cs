namespace Testcontainers.Tests;

public sealed class WithReuseTest
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ThrowsIfWithReuseIsNotLastCalledMethod()
    {
        Assert.Throws<ArgumentException>(() => new ContainerBuilder()
            .WithReuse(true)
            .WithImage(CommonImages.Alpine)
            .Build());
    }
}