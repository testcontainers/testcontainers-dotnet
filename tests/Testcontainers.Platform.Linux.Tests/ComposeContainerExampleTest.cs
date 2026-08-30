namespace Testcontainers.Tests;

public sealed class ComposeContainerExampleTest : IAsyncLifetime
{
    private const string WebServiceName = "web";

    private const string WorkerServiceName = "worker";

    private const ushort WebServicePort = 80;

    // # --8<-- [start:CreateComposeContainer]
    private readonly ComposeContainer _composeContainer;

    public ComposeContainerExampleTest()
    {
        var composeFileDirectoryPath = Directory.CreateDirectory(Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"))).FullName;

        var composeFilePath = Path.Combine(composeFileDirectoryPath, "compose.yml");

        File.WriteAllText(composeFilePath, $"""
            services:
              {WebServiceName}:
                image: "{CommonImages.Nginx.FullName}"
                ports:
                  - "{WebServicePort}"
              {WorkerServiceName}:
                image: "{CommonImages.Alpine.FullName}"
                command: ["/bin/sh", "-c", "echo Ready; sleep infinity"]
            """);

        var webServiceIsReady = Wait.ForUnixContainer()
            .UntilHttpRequestIsSucceeded(request => request.ForPath("/").ForPort(WebServicePort));

        var workerServiceIsReady = Wait.ForUnixContainer()
            .UntilMessageIsLogged("Ready");

        _composeContainer = new ComposeBuilder(CommonImages.DockerCli)
            .WithComposeFile(composeFilePath)
            .WithExposedService(WebServiceName, WebServicePort, webServiceIsReady)
            .WaitingFor(WorkerServiceName, workerServiceIsReady)
            .Build();
    }
    // # --8<-- [end:CreateComposeContainer]

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
        // # --8<-- [start:ConnectToExposedService]
        var serviceHost = _composeContainer.GetServiceHost(WebServiceName, WebServicePort);

        var servicePort = _composeContainer.GetServicePort(WebServiceName, WebServicePort);

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new UriBuilder(Uri.UriSchemeHttp, serviceHost, servicePort).Uri;

        using var httpResponse = await httpClient.GetAsync("/", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        // # --8<-- [end:ConnectToExposedService]

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task ReadsLogsOfNonExposedService()
    {
        // # --8<-- [start:GetServiceContainer]
        var workerContainer = _composeContainer.GetServiceContainer(WorkerServiceName);

        var (stdout, _) = await workerContainer.GetLogsAsync(ct: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        // # --8<-- [end:GetServiceContainer]

        Assert.Contains("Ready", stdout);
    }
}
