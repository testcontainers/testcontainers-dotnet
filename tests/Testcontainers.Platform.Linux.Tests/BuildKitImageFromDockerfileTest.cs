namespace Testcontainers.Tests;

using System.Security.Cryptography;
using System.Text.Json;
using DotNet.Testcontainers.Images;
using Microsoft.Extensions.Logging;

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

        // The build secret value does not appear in the Dockerfile. The Dockerfile
        // instructions become the history of the built image, which would report the
        // build secret value that this test asserts is not reported.
        var secretValueHash = BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(secretValue))).Replace("-", string.Empty).ToLowerInvariant();

        // The first instruction fails the build if the build secret is not readable,
        // the second one fails it if the build secret outlives the instruction that
        // mounts it: https://github.com/testcontainers/testcontainers-dotnet/issues/1406.
        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN --mount=type=secret,id={secretId} [ "$(sha256sum < /run/secrets/{secretId} | cut -d ' ' -f 1)" = "{secretValueHash}" ]
            RUN [ ! -e /run/secrets/{secretId} ]
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithSecret(secretId, secretValue)
            .Build();

        // When
        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder().Build();

        var imageHistoryResponse = await dockerClient.Images.GetImageHistoryAsync(image.FullName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var imageInspectResponse = await dockerClient.Images.InspectImageAsync(image.FullName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then

        // The image history reports the instruction that mounts the build secret. Its
        // hash confirms that the assertion below inspects the instructions, and does
        // not pass because the image history is empty.
        Assert.Contains(secretValueHash, JsonSerializer.Serialize(imageHistoryResponse));
        Assert.DoesNotContain(secretValue, JsonSerializer.Serialize(imageHistoryResponse));
        Assert.DoesNotContain(secretValue, JsonSerializer.Serialize(imageInspectResponse));
    }

    [Fact]
    public async Task RemovesDockerCliContainerWhenCleanUpIsDisabled()
    {
        // Given

        // The Docker CLI container is an implementation detail of the image build.
        // Disabling the cleanup keeps the built image, it does not keep the container
        // that built it, which carries the build secrets.
        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder().Build();

        // Derive a Docker CLI image for this test only. The ancestor filter resolves
        // the image id, so an image of its own makes the Docker CLI container of this
        // test the only container that the filter matches, independent of the image
        // builds that run at the same time.
        await using var cliImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CreateDockerfileDirectory($"""
                FROM {CommonImages.DockerCli.FullName}
                LABEL "org.testcontainers.docker-cli"="{Guid.NewGuid():D}"
                """))
            .Build();

        await cliImage.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN --mount=type=secret,id=mysecret [ -s /run/secrets/mysecret ]
            """);

        var image = new BuildKitImageFromDockerfileBuilder(cliImage)
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithSecret("mysecret", Guid.NewGuid().ToString("D"))
            .WithCleanUp(false)
            .Build();

        var imageName = string.Empty;

        try
        {
            await image.CreateAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            imageName = image.FullName;

            // When
            await image.DisposeAsync()
                .ConfigureAwait(true);

            var containerListParameters = new ContainersListParameters { All = true, Filters = new FilterByProperty().Add("ancestor", cliImage.FullName) };

            var containers = await dockerClient.Containers.ListContainersAsync(containerListParameters, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.Empty(containers);
        }
        finally
        {
            // Disabling the cleanup takes the built image out of the Resource Reaper
            // session, which makes this test responsible for it.
            if (!string.IsNullOrEmpty(imageName))
            {
                _ = await dockerClient.Images.DeleteImageAsync(imageName, new ImageDeleteParameters { Force = true }, TestContext.Current.CancellationToken)
                    .ConfigureAwait(true);
            }
        }
    }

    [Fact]
    public async Task ThrowsWhenDockerCliImageCannotBePulled()
    {
        // Given

        // A Docker CLI container that does not start is not necessarily a Docker
        // socket that cannot be mounted. The Docker daemon error propagates unchanged.
        var dockerfileDirectoryPath = CreateDockerfileDirectory($"FROM {CommonImages.Alpine.FullName}");

        await using var image = new BuildKitImageFromDockerfileBuilder("docker:0.0.0-does-not-exist-cli")
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .Build();

        // When
        var exception = await Assert.ThrowsAsync<DockerApiException>(() => image.CreateAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Contains("docker:0.0.0-does-not-exist-cli", exception.Message);
        Assert.DoesNotContain(nameof(TestcontainersSettings.DockerSocketOverride), exception.Message);
    }

    [Fact]
    public async Task BuildsForExpectedPlatform()
    {
        // Given
        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder().Build();

        var versionResponse = await dockerClient.System.GetVersionAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Build the image for a platform other than the platform of the Docker host.
        // The Dockerfile does not run an instruction, which keeps the build
        // independent of an emulator such as QEMU.
        var platform = "arm64".Equals(versionResponse.Arch, StringComparison.OrdinalIgnoreCase) ? "linux/amd64" : "linux/arm64";

        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            ENV TESTCONTAINERS_PLATFORM="{platform}"
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithPlatform(platform)
            .Build();

        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        var imageInspectResponse = await dockerClient.Images.InspectImageAsync(image.FullName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(platform, string.Join("/", imageInspectResponse.Os, imageInspectResponse.Architecture));
    }

    [Fact]
    public async Task BuildsFromContextDirectory()
    {
        // Given
        var contextDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        await File.WriteAllTextAsync(Path.Combine(contextDirectoryPath, "hello.txt"), "Hello, BuildKit!", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // The Dockerfile directory does not contain the file that the Dockerfile
        // copies. It is only part of the build context directory.
        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            COPY hello.txt /hello.txt
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithContextDirectory(contextDirectoryPath)
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .Build();

        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await using var container = CreateKeepAliveContainer(image);

        await container.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        var execResult = await container.ExecAsync(new[] { "cat", "/hello.txt" }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(0L, execResult.ExitCode);
        Assert.Equal("Hello, BuildKit!", execResult.Stdout);
    }

    [Fact]
    public async Task AppliesImageBuildParametersToBuildCommand()
    {
        // Given
        var fakeLogger = new FakeLogger();

        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN touch /build
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithLogger(fakeLogger)
            .WithCreateParameterModifier(parameters =>
            {
                parameters.NoCache = true;
                parameters.Pull = bool.TrueString;
                parameters.NetworkMode = "none";
                parameters.ShmSize = 67108864;
                parameters.ExtraHosts = new List<string> { "testcontainers.local:127.0.0.1" };
                parameters.CacheFrom = new List<string> { "type=local,src=/tmp/testcontainers/cache" };

                // The legacy builder squashes the layers of the built image. BuildKit
                // does not provide an equivalent.
                parameters.Squash = true;
            })
            .Build();

        // When
        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var logRecords = fakeLogger.Collector.GetSnapshot();

        var buildCommand = logRecords.Single(logRecord => logRecord.Message.StartsWith("Build Docker image ", StringComparison.Ordinal)).Message;

        // Then
        Assert.Contains("--no-cache", buildCommand);
        Assert.Contains("--pull", buildCommand);
        Assert.Contains("--network none", buildCommand);
        Assert.Contains("--shm-size 67108864", buildCommand);
        Assert.Contains("--add-host testcontainers.local:127.0.0.1", buildCommand);
        Assert.Contains("--cache-from type=local,src=/tmp/testcontainers/cache", buildCommand);
        Assert.Contains(logRecords, logRecord => logRecord.Level == LogLevel.Warning && logRecord.Message.Contains(nameof(ImageBuildParameters.Squash), StringComparison.Ordinal));
    }

    [Fact]
    public async Task BuildsImageWhenImageBuildParameterCollectionsAreReset()
    {
        // Given
        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder().Build();

        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            RUN touch /build
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithBuildArgument("MAGIC_NUMBER", "42")
            .WithCreateParameterModifier(parameters =>
            {
                // A parameter modifier can reset a collection of the image build
                // parameters. The Docker CLI command does not enumerate it then.
                parameters.BuildArgs = null;
                parameters.Labels = null;
            })
            .Build();

        // When
        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var imageInspectResponse = await dockerClient.Images.InspectImageAsync(image.FullName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.NotNull(imageInspectResponse);
    }

    [Fact]
    public async Task LogsRedactedBuildCommandAndBuildOutputAtDebugLevel()
    {
        // Given
        var fakeLogger = new FakeLogger();

        var buildArgumentValue = Guid.NewGuid().ToString("D");

        var dockerfileDirectoryPath = CreateDockerfileDirectory($"""
            FROM {CommonImages.Alpine.FullName}
            ARG TOKEN
            """);

        await using var image = new BuildKitImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfileDirectoryPath)
            .WithBuildArgument("TOKEN", buildArgumentValue)
            .WithLogger(fakeLogger)
            .Build();

        // When
        await image.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var logRecords = fakeLogger.Collector.GetSnapshot();

        // Then
        Assert.Contains(logRecords, logRecord => logRecord.Level == LogLevel.Debug && logRecord.Message.Contains("--build-arg TOKEN=***", StringComparison.Ordinal));
        Assert.Contains(logRecords, logRecord => logRecord.Level == LogLevel.Debug && logRecord.Message.Contains("build output:", StringComparison.Ordinal));
        Assert.DoesNotContain(logRecords, logRecord => logRecord.Message.Contains(buildArgumentValue, StringComparison.Ordinal));
        Assert.DoesNotContain(logRecords, logRecord => logRecord.Level > LogLevel.Debug && logRecord.Message.Contains("buildx build", StringComparison.Ordinal));
        Assert.DoesNotContain(logRecords, logRecord => logRecord.Level > LogLevel.Debug && logRecord.Message.Contains("build output:", StringComparison.Ordinal));
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
    private readonly string _dockerSocketOverride = TestcontainersSettings.DockerSocketOverride;

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        TestcontainersSettings.DockerSocketOverride = _dockerSocketOverride;
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
