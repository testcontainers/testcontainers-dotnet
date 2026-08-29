namespace Testcontainers.Tests;

public sealed class ComposeContainerTest : IAsyncLifetime
{
    private const string ServiceName = "web";

    private const ushort ServicePort = 80;

    private readonly ComposeContainer _composeContainer;

    public ComposeContainerTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        // The service publishes its port to a random host port to cover the readiness
        // check that waits until the port bindings are mapped.
        File.WriteAllText(composeFilePath, $"services:\n  {ServiceName}:\n    image: \"{CommonImages.Nginx.FullName}\"\n    ports:\n      - \"{ServicePort}\"\n");

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WithExposedService(ServiceName, ServicePort, Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(ServicePort)))
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task EstablishesConnectionToExposedService()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new UriBuilder(Uri.UriSchemeHttp, _composeContainer.GetServiceHost(ServiceName, ServicePort), _composeContainer.GetServicePort(ServiceName, ServicePort)).Uri;

        // When
        using var httpResponse = await httpClient.GetAsync("/", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public void ReturnsServiceContainer()
    {
        // Given
        var serviceContainer = _composeContainer.GetServiceContainer(ServiceName);

        // When
        var mappedPublicPort = serviceContainer.GetMappedPublicPort(ServicePort);

        // Then
        Assert.Equal(TestcontainersStates.Running, serviceContainer.State);
        Assert.Equal(_composeContainer.GetServicePort(ServiceName, ServicePort), mappedPublicPort);
    }
}

public sealed class ComposeContainerExitedServiceTest
{
    private static ComposeContainer BuildComposeContainer(int exitCode)
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"services:\n  migration:\n    image: \"{CommonImages.Alpine.FullName}\"\n    command: [\"/bin/sh\", \"-c\", \"exit {exitCode}\"]\n");

        return new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .Build();
    }

    [Fact]
    public async Task StartsWhenServiceRanToCompletion()
    {
        // Given
        await using var composeContainer = BuildComposeContainer(0);

        // When
        await composeContainer.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(TestcontainersStates.Exited, composeContainer.GetServiceContainer("migration").State);
    }

    [Fact]
    public async Task ThrowsWhenServiceExitedUnsuccessfully()
    {
        // Given
        await using var composeContainer = BuildComposeContainer(1);

        // When
        var exception = await Assert.ThrowsAsync<ContainerNotRunningException>(() => composeContainer.StartAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Contains("exited with code 1", exception.Message);
    }
}

public sealed class ComposeContainerFileCopyInclusionTest : IAsyncLifetime
{
    private readonly ComposeContainer _composeContainer;

    private readonly string _composeFileDirectoryPath;

    public ComposeContainerFileCopyInclusionTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        _composeFileDirectoryPath = composeFileDirectoryPath;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"services:\n  migration:\n    image: \"{CommonImages.Alpine.FullName}\"\n    command: [\"/bin/sh\", \"-c\", \"exit 0\"]\n");

        _ = Directory.CreateDirectory(Path.Combine(composeFileDirectoryPath, "included"));
        File.WriteAllText(Path.Combine(composeFileDirectoryPath, "included", "included-nested.txt"), string.Empty);
        File.WriteAllText(Path.Combine(composeFileDirectoryPath, "included.txt"), string.Empty);
        File.WriteAllText(Path.Combine(composeFileDirectoryPath, "excluded.txt"), string.Empty);

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WithCopyFilesInContainer("included.txt", "included")
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task CopiesIncludedFilesOnly()
    {
        // Given
        var execResult = await _composeContainer.ExecAsync(new[] { "find", _composeFileDirectoryPath, "-type", "f" }, TestContext.Current.CancellationToken)
            .ThrowOnFailure()
            .ConfigureAwait(true);

        // When
        var filePaths = execResult.Stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(filePath => filePath.Trim()).ToArray();

        // Then
        Assert.Contains(Path.Combine(_composeFileDirectoryPath, "compose.yml"), filePaths);
        Assert.Contains(Path.Combine(_composeFileDirectoryPath, "included.txt"), filePaths);
        Assert.Contains(Path.Combine(_composeFileDirectoryPath, "included", "included-nested.txt"), filePaths);
        Assert.DoesNotContain(Path.Combine(_composeFileDirectoryPath, "excluded.txt"), filePaths);
    }
}

public sealed class ComposeContainerRelativeBindMountTest : IAsyncLifetime
{
    private const string FileContent = "Docker Compose resolves the bind mount source on the test host.";

    private readonly ComposeContainer _composeContainer;

    public ComposeContainerRelativeBindMountTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        _ = Directory.CreateDirectory(Path.Combine(composeFileDirectoryPath, "data"));
        File.WriteAllText(Path.Combine(composeFileDirectoryPath, "data", "bind-mount.txt"), FileContent);

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"services:\n  app:\n    image: \"{CommonImages.Alpine.FullName}\"\n    volumes:\n      - \"./data:/data\"\n    command: [\"/bin/sh\", \"-c\", \"sleep infinity\"]\n");

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task ResolvesRelativeBindMountToTestHostPath()
    {
        // Given
        var serviceContainer = _composeContainer.GetServiceContainer("app");

        // When
        var fileContent = await serviceContainer.ReadFileAsync("/data/bind-mount.txt", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(FileContent, Encoding.Default.GetString(fileContent));
    }
}

public sealed class ComposeContainerPathContainingPathSeparatorTest : IAsyncLifetime
{
    private const string ServiceName = "app";

    // A colon is a valid character in a Unix path, but it is the default path
    // separator that Docker Compose splits COMPOSE_FILE on. Windows does not allow
    // it in a path, there a Docker Compose file path cannot contain it.
    private static readonly string DirectoryNameSuffix = OperatingSystem.IsWindows() ? string.Empty : ":1";

    private readonly ComposeContainer _composeContainer;

    public ComposeContainerPathContainingPathSeparatorTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D") + DirectoryNameSuffix)).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"services:\n  {ServiceName}:\n    image: \"{CommonImages.Alpine.FullName}\"\n    command: [\"/bin/sh\", \"-c\", \"sleep infinity\"]\n");

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public void StartsWhenComposeFilePathContainsPathSeparator()
    {
        // Given
        var serviceContainer = _composeContainer.GetServiceContainer(ServiceName);

        // When
        var state = serviceContainer.State;

        // Then
        Assert.Equal(TestcontainersStates.Running, state);
    }
}

public sealed class ComposeContainerWaitingForTest : IAsyncLifetime
{
    private readonly ComposeContainer _composeContainer;

    public ComposeContainerWaitingForTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        // The service does not publish or expose a port. Its readiness is only
        // observable through its log message.
        File.WriteAllText(composeFilePath, $"services:\n  app:\n    image: \"{CommonImages.Alpine.FullName}\"\n    command: [\"/bin/sh\", \"-c\", \"sleep 1; echo Ready; sleep infinity\"]\n");

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WaitingFor("app", Wait.ForUnixContainer().UntilMessageIsLogged("Ready"))
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task WaitsForServiceWithoutExposedPort()
    {
        // Given
        var serviceContainer = _composeContainer.GetServiceContainer("app");

        // When
        var (stdout, _) = await serviceContainer.GetLogsAsync(ct: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Contains("Ready", stdout);
        Assert.Throws<ComposeServiceNotExposedException>(() => _composeContainer.GetServicePort("app", 80));
    }
}

public sealed class ComposeContainerServiceNameWithInstanceSuffixTest : IAsyncLifetime
{
    // A Docker Compose service name may end with a dash and a number. It addresses
    // the service that carries the name, not the second instance of a service that
    // is named "web".
    private const string ServiceName = "web-2";

    private readonly ComposeContainer _composeContainer;

    public ComposeContainerServiceNameWithInstanceSuffixTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"services:\n  {ServiceName}:\n    image: \"{CommonImages.Alpine.FullName}\"\n    command: [\"/bin/sh\", \"-c\", \"echo Ready; sleep infinity\"]\n");

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WaitingFor(ServiceName, Wait.ForUnixContainer().UntilMessageIsLogged("Ready"))
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public void ResolvesTheServiceNameThatEndsWithAnInstanceNumber()
    {
        Assert.Equal(TestcontainersStates.Running, _composeContainer.GetServiceContainer(ServiceName).State);
    }
}

public sealed class ComposeContainerServiceNotFoundTest
{
    [Fact]
    public async Task ThrowsWhenWaitStrategyReferencesUnknownService()
    {
        // Given
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"services:\n  app:\n    image: \"{CommonImages.Alpine.FullName}\"\n    command: [\"/bin/sh\", \"-c\", \"sleep infinity\"]\n");

        await using var composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WaitingFor("aap", Wait.ForUnixContainer().UntilMessageIsLogged("Ready"))
            .Build();

        // When
        var exception = await Assert.ThrowsAsync<ComposeServiceNotFoundException>(() => composeContainer.StartAsync(TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Contains("'aap-1'", exception.Message);
    }
}

public sealed class ComposeContainerPullTest : IAsyncLifetime
{
    private readonly FakeLogger _fakeLogger = new FakeLogger();

    private readonly ComposeContainer _composeContainer;

    public ComposeContainerPullTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        File.WriteAllText(Path.Combine(composeFileDirectoryPath, "Dockerfile"), $"FROM {CommonImages.Alpine.FullName}\nCMD [\"/bin/sh\", \"-c\", \"sleep infinity\"]\n");

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        // Docker Compose builds the image of this service. It does not exist in a
        // registry, so pulling it fails. That must not fail the start.
        File.WriteAllText(composeFilePath, "services:\n  app:\n    build: \".\"\n");

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WithLogger(_fakeLogger)
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public void StartsWhenImageCannotBePulled()
    {
        // Given
        var logRecords = _fakeLogger.Collector.GetSnapshot();

        // When
        var pullImageFailed = logRecords.Any(logRecord => logRecord.Message.Contains("Cannot pull the Docker Compose image"));

        // Then
        Assert.True(pullImageFailed, "Expected a warning about the image that cannot be pulled.");
        Assert.Equal(TestcontainersStates.Running, _composeContainer.GetServiceContainer("app").State);
    }
}

public sealed class ComposeContainerScaledServiceTest : IAsyncLifetime
{
    private const string ServiceName = "web";

    private const ushort ServicePort = 80;

    private const ushort Instances = 2;

    private readonly ComposeContainer _composeContainer;

    public ComposeContainerScaledServiceTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"services:\n  {ServiceName}:\n    image: \"{CommonImages.Nginx.FullName}\"\n");

        var composeBuilder = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WithScaledService(ServiceName, Instances);

        // Each instance gets its own ambassador port, and each instance runs its own
        // readiness check.
        for (ushort instance = 1; instance <= Instances; instance++)
        {
            composeBuilder = composeBuilder.WithExposedServiceInstance(ServiceName, instance, ServicePort);
        }

        _composeContainer = composeBuilder.Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _composeContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _composeContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public void ResolvesEachServiceInstanceIndividually()
    {
        // Given
        var containerIds = Enumerable.Range(1, Instances)
            .Select(instance => _composeContainer.GetServiceInstanceContainer(ServiceName, (ushort)instance).Id)
            .ToArray();

        // When
        var servicePorts = Enumerable.Range(1, Instances)
            .Select(instance => _composeContainer.GetServiceInstancePort(ServiceName, (ushort)instance, ServicePort))
            .ToArray();

        // Then
        Assert.Equal(Instances, containerIds.Distinct().Count());
        Assert.Equal(Instances, servicePorts.Distinct().Count());
    }

    [Fact]
    public void ResolvesTheServiceNameToTheFirstInstance()
    {
        Assert.Equal(_composeContainer.GetServiceInstanceContainer(ServiceName, 1).Id, _composeContainer.GetServiceContainer(ServiceName).Id);
        Assert.Equal(_composeContainer.GetServiceInstancePort(ServiceName, 1, ServicePort), _composeContainer.GetServicePort(ServiceName, ServicePort));
    }

    [Theory]
    [InlineData((ushort)1)]
    [InlineData((ushort)2)]
    public async Task EstablishesConnectionToEachServiceInstance(ushort instance)
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new UriBuilder(Uri.UriSchemeHttp, _composeContainer.GetServiceInstanceHost(ServiceName, instance, ServicePort), _composeContainer.GetServiceInstancePort(ServiceName, instance, ServicePort)).Uri;

        // When
        using var httpResponse = await httpClient.GetAsync("/", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }
}
