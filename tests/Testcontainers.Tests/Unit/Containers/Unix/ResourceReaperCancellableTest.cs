namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  // NOTICE: These tests will produce `SocketException` messages in the log, since there is no listening port the ResourceReaper could connect to.
  // Any Docker image that listen on 8080 would hide the exceptions. The tests will still work, as long as the ResourceReaper does not get the acknowledgment.
  public sealed class ResourceReaperCancellableTest : IDisposable
  {
    private readonly Guid sessionId = Guid.NewGuid();

    private readonly CancellationTokenSource cts = new CancellationTokenSource();

    private readonly IList<ResourceReaperState> stateChanges = new List<ResourceReaperState>();

    public ResourceReaperCancellableTest()
    {
      ResourceReaper.StateChanged += this.OnStateChanged;
    }

    [Fact]
    public async Task DisposeAsyncShouldAwaitConnectionTerminatedState()
    {
      // Given
      var resourceReaper = await ResourceReaper.GetAndStartNewAsync(this.sessionId)
        .ConfigureAwait(false);

      // When
      await resourceReaper.DisposeAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(Enum.GetValues(typeof(ResourceReaperState)).Cast<ResourceReaperState>(), this.stateChanges);
    }

    [Fact]
    public async Task ResourceReaperShouldTimeoutIfInitializationFails()
    {
      var resourceReaperTask = ResourceReaper.GetAndStartNewAsync(this.sessionId, null, "nginx", TimeSpan.FromSeconds(10));
      _ = await Assert.ThrowsAsync<ResourceReaperException>(() => resourceReaperTask);
      Assert.Equal(new[] { ResourceReaperState.Created, ResourceReaperState.InitializingConnection }, this.stateChanges);
    }

    [Fact]
    public async Task GetAndStartNewAsyncShouldBeCancellableDuringContainerStart()
    {
      ResourceReaper.StateChanged += this.CancelOnCreated;

      var resourceReaperTask = ResourceReaper.GetAndStartNewAsync(this.sessionId, null, "nginx", TimeSpan.FromSeconds(10), this.cts.Token);
      _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => resourceReaperTask);
      Assert.Equal(new[] { ResourceReaperState.Created }, this.stateChanges);
    }

    [Fact]
    public async Task GetAndStartNewAsyncShouldBeCancellableDuringInitializingConnection()
    {
      ResourceReaper.StateChanged += this.CancelOnInitializingConnection;

      var resourceReaperTask = ResourceReaper.GetAndStartNewAsync(this.sessionId, null, "nginx", TimeSpan.FromSeconds(10), this.cts.Token);
      _ = await Assert.ThrowsAsync<ResourceReaperException>(() => resourceReaperTask);
      Assert.Equal(new[] { ResourceReaperState.Created, ResourceReaperState.InitializingConnection }, this.stateChanges);
    }

    public void Dispose()
    {
      ResourceReaper.StateChanged -= this.OnStateChanged;
      ResourceReaper.StateChanged -= this.CancelOnCreated;
      ResourceReaper.StateChanged -= this.CancelOnInitializingConnection;
      ResourceReaper.StateChanged -= this.CancelOnMaintainingConnection;
      this.cts.Dispose();
    }

    private void OnStateChanged(object sender, ResourceReaperStateEventArgs e)
    {
      if (this.IsTest(e.Instance))
      {
        this.stateChanges.Add(e.State);
      }
    }

    private void CancelOnCreated(object sender, ResourceReaperStateEventArgs e)
    {
      if (this.IsTest(e.Instance) && ResourceReaperState.Created.Equals(e.State))
      {
        this.cts.Cancel();
      }
    }

    private void CancelOnInitializingConnection(object sender, ResourceReaperStateEventArgs e)
    {
      if (this.IsTest(e.Instance) && ResourceReaperState.InitializingConnection.Equals(e.State))
      {
        this.cts.CancelAfter(TimeSpan.FromSeconds(1));
      }
    }

    private void CancelOnMaintainingConnection(object sender, ResourceReaperStateEventArgs e)
    {
      if (this.IsTest(e.Instance) && ResourceReaperState.MaintainingConnection.Equals(e.State))
      {
        this.cts.CancelAfter(TimeSpan.FromSeconds(1));
      }
    }

    private bool IsTest(ResourceReaper resourceReaper)
    {
      return resourceReaper.SessionId.Equals(this.sessionId);
    }
  }
}
