namespace Testcontainers.EventStoreDb;

public sealed class EventStoreDbContainerTest : ContainerTest<EventStoreDbBuilder, EventStoreDbContainer>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReadStreamReturnsEvent()
    {
        // Given
        const string eventType = "some-event";

        const string streamName = "some-stream";

        var settings = EventStoreClientSettings.Create(Container.GetConnectionString());

        using var client = new EventStoreClient(settings);

        var eventData = new EventData(Uuid.NewUuid(), eventType, Array.Empty<byte>());

        // When
        _ = await client.AppendToStreamAsync(streamName, StreamState.NoStream, new[] { eventData })
            .ConfigureAwait(false);

        var resolvedEvents = client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start);

        var resolvedEvent = await resolvedEvents.FirstAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(eventType, resolvedEvent.Event.EventType);
    }
}