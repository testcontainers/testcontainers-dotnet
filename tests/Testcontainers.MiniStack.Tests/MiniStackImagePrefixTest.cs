namespace Testcontainers.MiniStack;

// Changes the process-wide hub image name prefix, so it runs on its own.
[CollectionDefinition(nameof(MiniStackImagePrefixTest), DisableParallelization = true)]
public sealed class MiniStackImagePrefixCollection;

[Collection(nameof(MiniStackImagePrefixTest))]
public sealed class MiniStackImagePrefixTest
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HubImageNamePrefixIsPassedToMiniStack()
    {
        // Given
        const string hubImageNamePrefix = "docker.io/";

        var previousHubImageNamePrefix = TestcontainersSettings.HubImageNamePrefix;

        TestcontainersSettings.HubImageNamePrefix = hubImageNamePrefix;

        try
        {
            await using var miniStackContainer = new MiniStackBuilder(TestSession.GetImageFromDockerfile()).Build();

            // When
            await miniStackContainer.StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            var execResult = await miniStackContainer.ExecAsync(new[] { "printenv", "MINISTACK_IMAGE_PREFIX" }, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.Equal(hubImageNamePrefix, execResult.Stdout.Trim());
        }
        finally
        {
            TestcontainersSettings.HubImageNamePrefix = previousHubImageNamePrefix;
        }
    }
}
