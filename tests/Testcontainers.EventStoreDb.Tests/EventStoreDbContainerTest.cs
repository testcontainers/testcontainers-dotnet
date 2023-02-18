namespace Testcontainers.EventStore;

public sealed class EventStoreDbContainerTest : IAsyncLifetime
{
  private readonly EventStoreDbContainer _eventStoreContainer = new EventStoreDbBuilder().Build();

  public Task InitializeAsync()
  {
    return _eventStoreContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _eventStoreContainer.DisposeAsync().AsTask();
  }

  [Fact]
  public async Task ConnectionEstablished()
  {
    // Given
    var settings = EventStoreClientSettings.Create(_eventStoreContainer.GetConnectionString());
    using EventStoreClient connection = new EventStoreClient(settings);

    // When
    var tokenSource = new CancellationTokenSource();
    var cancellationToken = tokenSource.Token;

    var evt = new { EntityId = Guid.NewGuid().ToString("N"), ImportantData = "I wrote my first event!" };

    var eventData = new EventData(
      Uuid.NewUuid(),
      "TestEvent",
      JsonSerializer.SerializeToUtf8Bytes(evt));

    await connection.AppendToStreamAsync(
      "some-stream",
      StreamState.Any,
      new[] { eventData },
      cancellationToken: cancellationToken);

    var result = connection.ReadStreamAsync(
      Direction.Forwards,
      "some-stream",
      StreamPosition.Start,
      cancellationToken: cancellationToken);

    var events = await result.ToListAsync(cancellationToken);

    // Then
    Assert.NotNull(events);
  }
}
