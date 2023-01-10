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
  using StreamPosition = EventStore.ClientAPI.StreamPosition;

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

      // // When

      CancellationTokenSource tokenSource = new CancellationTokenSource();
      CancellationToken cancellationToken = tokenSource.Token;

      #region createEvent

      var evt = new { EntityId = Guid.NewGuid().ToString("N"), ImportantData = "I wrote my first event!" };

      var eventData = new EventData(
        Uuid.NewUuid(),
        "TestEvent",
        JsonSerializer.SerializeToUtf8Bytes(evt)
      );

      #endregion createEvent

      try
      {
        #region appendEvents

        await connection.AppendToStreamAsync(
          "some-stream",
          StreamState.Any,
          new[] { eventData },
          cancellationToken: cancellationToken);

        #endregion appendEvents
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }

      #region overriding-user-credentials

      await connection.AppendToStreamAsync(
        "some-stream",
        StreamState.Any,
        new[] { eventData },
        userCredentials: new UserCredentials("admin", "changeit"),
        cancellationToken: cancellationToken
      );

      #endregion overriding-user-credentials

      #region readStream

      var result = connection.ReadStreamAsync(
        Direction.Forwards,
        "some-stream",
        StreamPosition.Start,
        cancellationToken: cancellationToken);

      var events = await result.ToListAsync(cancellationToken);

      #endregion readStream


      // var data = Encoding.UTF8.GetBytes("some data");
      // var metadata = Encoding.UTF8.GetBytes("some metadata");
      // var evt = new EventStore.ClientAPI.EventData(Guid.NewGuid(), "testEvent", false, data, metadata);
      //
      // await connection.AppendToStreamAsync("test-stream", ExpectedVersion.Any, evt);
      //
      // var streamEvents = await connection.ReadStreamEventsForwardAsync("test-stream", 0, 1, false);
      // var returnedEvent = streamEvents.Events[0].Event;
      //
      // Console.WriteLine(
      //   "Read event with data: {0}, metadata: {1}",
      //   Encoding.UTF8.GetString(returnedEvent.Data),
      //   Encoding.UTF8.GetString(returnedEvent.Metadata)
      // );

      // Then
      Assert.NotNull(events);
    }
  }
}
