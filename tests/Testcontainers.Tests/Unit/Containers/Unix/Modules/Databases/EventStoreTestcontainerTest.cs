namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Linq;
  using System.Text.Json;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using EventStore.Client;
  using Xunit;

  public sealed class EventStoreTestcontainerTest : IClassFixture<EventStoreFixture>
  {
    private readonly EventStoreFixture eventStoreFixture;

    public EventStoreTestcontainerTest(EventStoreFixture eventStoreFixture)
    {
      this.eventStoreFixture = eventStoreFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.eventStoreFixture.Connection;

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
}
