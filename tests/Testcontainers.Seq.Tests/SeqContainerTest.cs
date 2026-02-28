namespace Testcontainers.Seq;

public sealed class SeqContainerTest : IAsyncLifetime
{
    private readonly SeqContainer _seqContainer = new SeqBuilder(TestSession.GetImageFromDockerfile()).WithAcceptLicenseAgreement(true).Build();

    public async ValueTask InitializeAsync()
    {
        await _seqContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _seqContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task LogsMessageToSeq()
    {
        // Given
        const string helloWorld = "Hello, World!";

        var endpoint = _seqContainer.GetEndpoint();

        var loggerFactory = new LoggerFactory();
        loggerFactory.AddSeq(endpoint);

        var logger = loggerFactory.CreateLogger(nameof(SeqContainerTest));
        logger.LogInformation(helloWorld);

        // Ensure pending messages are sent before querying.
        loggerFactory.Dispose();

        using var connection = new SeqConnection(endpoint);

        // When
        var events = await connection.Events.ListAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Single(events);
        Assert.Equal(helloWorld, events[0].MessageTemplateTokens.Last().Text);
    }
}