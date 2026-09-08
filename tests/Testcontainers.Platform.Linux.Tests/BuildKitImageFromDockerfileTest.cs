namespace Testcontainers.Tests;

using DotNet.Testcontainers.Images;

public sealed class BuildKitImageFromDockerfileTest
{
    [Fact]
    public async Task BuildsHeredocDockerfile()
    {
        // Given

        // The legacy builder does not interpret a here-document. It creates an empty
        // file, and the container that runs it fails with an exec format error:
        // https://github.com/testcontainers/testcontainers-dotnet/issues/1247.
        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN cat <<EOF > /entrypoint.sh
            #!/bin/sh
            echo "Hello, BuildKit!"
            EOF
            RUN chmod +x /entrypoint.sh
            ENTRYPOINT ["/entrypoint.sh"]
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .Build();

        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await using var container = CreateKeepAliveContainer(image);

        await container.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        var readEntrypointResult = await container.ExecAsync(new[] { "cat", "/entrypoint.sh" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var runEntrypointResult = await container.ExecAsync(new[] { "/entrypoint.sh" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(0L, readEntrypointResult.ExitCode);
        Assert.StartsWith("#!/bin/sh", readEntrypointResult.Stdout);
        Assert.Contains("echo \"Hello, BuildKit!\"", readEntrypointResult.Stdout);
        Assert.Equal(0L, runEntrypointResult.ExitCode);
        Assert.Contains("Hello, BuildKit!", runEntrypointResult.Stdout);
    }

    [Fact]
    public async Task MountsBuildSecret()
    {
        // Given
        const string secretId = "mysecret";

        var secretValue = Guid.NewGuid().ToString("D");

        // The first instruction fails the build if the build secret is not readable,
        // the second one fails it if the build secret outlives the instruction that
        // mounts it: https://github.com/testcontainers/testcontainers-dotnet/issues/1406.
        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN --mount=type=secret,id={secretId} [ "$(cat /run/secrets/{secretId})" = "{secretValue}" ]
            RUN [ ! -e /run/secrets/{secretId} ]
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithSecret(secretId, secretValue)
            .Build();

        // When
        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await using var container = CreateKeepAliveContainer(image);

        await container.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var execResult = await container.ExecAsync(new[] { "ls", "/run/secrets/" + secretId }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.NotEqual(0L, execResult.ExitCode);
    }

    [Fact]
    public async Task MountsBuildSecretFromFile()
    {
        // Given
        const string secretId = "mysecret";

        var secretValue = Guid.NewGuid().ToString("D");

        var secretFilePath = Path.Combine(Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName, secretId);

        await File.WriteAllTextAsync(secretFilePath, secretValue, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN --mount=type=secret,id={secretId} [ "$(cat /run/secrets/{secretId})" = "{secretValue}" ]
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithSecret(secretId, new FileInfo(secretFilePath))
            .Build();

        // When
        var exception = await Record.ExceptionAsync(() => image.CreateAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Null(exception);
    }

    [Fact]
    public async Task AppliesLabelsAndBuildArgumentsToImage()
    {
        // Given
        var buildArgumentValue = Guid.NewGuid().ToString("D");

        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            ARG MAGIC_NUMBER="0"
            LABEL "org.testcontainers.magic-number"=$MAGIC_NUMBER
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithBuildArgument("MAGIC_NUMBER", buildArgumentValue)
            .WithLabel("org.testcontainers.buildkit", bool.TrueString.ToLowerInvariant())
            .Build();

        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder().Build();

        // When
        var imageInspectResponse = await dockerClient.Images.InspectImageAsync(image.FullName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(buildArgumentValue, imageInspectResponse.Config.Labels["org.testcontainers.magic-number"]);
        Assert.Equal(bool.TrueString.ToLowerInvariant(), imageInspectResponse.Config.Labels["org.testcontainers.buildkit"]);
        Assert.Contains(ResourceReaper.ResourceReaperSessionLabel, imageInspectResponse.Config.Labels.Keys);
    }

    [Fact]
    public async Task BuildsUpToExpectedTarget()
    {
        // Given
        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName} AS build
            RUN touch /build

            FROM build AS final
            RUN touch /final
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithTarget("build")
            .Build();

        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await using var container = CreateKeepAliveContainer(image);

        await container.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        var execResult = await container.ExecAsync(new[] { "ls", "/final" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.NotEqual(0L, execResult.ExitCode);
    }

    [Fact]
    public async Task BuildFailureIncludesDetailedErrorMessage()
    {
        // Given
        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN command-that-does-not-exist
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .Build();

        // When
        var exception = await Assert.ThrowsAsync<ImageBuildFailedException>(() => image.CreateAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.StartsWith("Docker image ", exception.Message);
        Assert.Contains(" has not been created.", exception.Message);
        Assert.Contains("command-that-does-not-exist", exception.Message);
    }

    private static string CreateDockerfileDirectory(string dockerfile)
    {
        var dockerfileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;
        File.WriteAllText(Path.Combine(dockerfileDirectoryPath, "Dockerfile"), dockerfile);
        return dockerfileDirectoryPath;
    }

    private static IContainer CreateKeepAliveContainer(IImage image)
    {
        // The images that the tests build either exit immediately or do not have an
        // entrypoint at all. Keep the container running, so that the tests can inspect
        // the image content.
        return new ContainerBuilder()
            .WithImage(image)
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("trap 'exit 0' TERM; sleep infinity & wait $!")
            .Build();
    }
}

[CollectionDefinition(nameof(DockerSocketOverrideCollection), DisableParallelization = true)]
public static class DockerSocketOverrideCollection
{
}

[Collection(nameof(DockerSocketOverrideCollection))]
public sealed class BuildKitImageFromDockerfileDockerSocketTest : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        TestcontainersSettings.DockerSocketOverride = null;
        _disposed = true;
    }

    [Fact]
    public async Task ThrowsWhenDockerSocketIsNotAvailable()
    {
        // Given

        // A Docker daemon that does not listen on a Unix socket, such as a Docker
        // daemon that is reached over a Windows named pipe, cannot provide a Docker
        // socket to bind-mount into the Docker CLI container.
        TestcontainersSettings.DockerSocketOverride = "/var/run/docker-socket-does-not-exist.sock";

        var dockerfileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        await File.WriteAllTextAsync(Path.Combine(dockerfileDirectoryPath, "Dockerfile"), $"FROM {CommonImages.Alpine.FullName}", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .Build();

        // When
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => image.CreateAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Contains("/var/run/docker-socket-does-not-exist.sock", exception.Message);
        Assert.Contains(nameof(TestcontainersSettings.DockerSocketOverride), exception.Message);
    }
}
