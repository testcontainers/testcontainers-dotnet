namespace Testcontainers.KurrentDb;

public sealed class KurrentDbContainerTest : IAsyncLifetime
{
    private readonly KurrentDbContainer _kurrentDbContainer = new KurrentDbBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _kurrentDbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _kurrentDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReadStreamReturnsEvent()
    {
        // Given
        const string eventType = "some-event";

        const string streamName = "some-stream";

        var settings = EventStoreClientSettings.Create(_kurrentDbContainer.GetConnectionString());

        using var client = new EventStoreClient(settings);

        var eventData = new EventData(Uuid.NewUuid(), eventType, Array.Empty<byte>());

        // When
        _ = await client.AppendToStreamAsync(streamName, StreamState.NoStream, new[] { eventData }, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var resolvedEvents = client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, cancellationToken: TestContext.Current.CancellationToken);

        var resolvedEvent = await resolvedEvents.FirstAsync(cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(eventType, resolvedEvent.Event.EventType);
    }
}