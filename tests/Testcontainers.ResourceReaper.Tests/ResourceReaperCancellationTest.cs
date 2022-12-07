namespace DotNet.Testcontainers.ResourceReaper.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class ResourceReaperCancellationTest : IAsyncLifetime, IDisposable
  {
    private readonly Guid id = Guid.NewGuid();

    private readonly CancellationTokenSource cts = new CancellationTokenSource();

    private readonly IList<ResourceReaperState> stateChanges = new List<ResourceReaperState>();

    public Task InitializeAsync()
    {
      ResourceReaper.StateChanged += this.OnStateChanged;
      return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
      ResourceReaper.StateChanged -= this.OnStateChanged;
      ResourceReaper.StateChanged -= this.CancelOnCreated;
      ResourceReaper.StateChanged -= this.CancelOnInitializingConnection;
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      this.cts.Dispose();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
    public async Task ResourceReaperRaisesStateChangesOnSuccessfulStart()
    {
      // Given
      var resourceReaper = await ResourceReaper.GetAndStartNewAsync(this.id, TestcontainersSettings.OS.DockerEndpointAuthConfig, CommonImages.Ryuk, ResourceReaper.UnixSocketMount.Instance)
        .ConfigureAwait(false);

      // When
      await resourceReaper.DisposeAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(Enum.GetValues(typeof(ResourceReaperState)).Cast<ResourceReaperState>(), this.stateChanges);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
    public async Task ResourceReaperRaisesStateChangesOnTimeout()
    {
      var resourceReaperTask = ResourceReaper.GetAndStartNewAsync(this.id, TestcontainersSettings.OS.DockerEndpointAuthConfig, CommonImages.Nginx, ResourceReaper.UnixSocketMount.Instance, false, TimeSpan.FromSeconds(10), this.cts.Token);
      _ = await Assert.ThrowsAsync<ResourceReaperException>(() => resourceReaperTask);
      Assert.Equal(new[] { ResourceReaperState.Created, ResourceReaperState.InitializingConnection }, this.stateChanges);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
    public async Task ResourceReaperCancelsStartOnCreated()
    {
      ResourceReaper.StateChanged += this.CancelOnCreated;

      var resourceReaperTask = ResourceReaper.GetAndStartNewAsync(this.id, TestcontainersSettings.OS.DockerEndpointAuthConfig, CommonImages.Nginx, ResourceReaper.UnixSocketMount.Instance, false, TimeSpan.FromSeconds(10), this.cts.Token);
      _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => resourceReaperTask);
      Assert.Equal(new[] { ResourceReaperState.Created }, this.stateChanges);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
    public async Task ResourceReaperCancelsStartOnInitializingConnection()
    {
      ResourceReaper.StateChanged += this.CancelOnInitializingConnection;

      var resourceReaperTask = ResourceReaper.GetAndStartNewAsync(this.id, TestcontainersSettings.OS.DockerEndpointAuthConfig, CommonImages.Nginx, ResourceReaper.UnixSocketMount.Instance, false, TimeSpan.FromSeconds(10), this.cts.Token);
      _ = await Assert.ThrowsAsync<ResourceReaperException>(() => resourceReaperTask);
      Assert.Equal(new[] { ResourceReaperState.Created, ResourceReaperState.InitializingConnection }, this.stateChanges);
    }

    private void OnStateChanged(object sender, ResourceReaperStateEventArgs e)
    {
      if (this.id.Equals(e.Instance.SessionId))
      {
        this.stateChanges.Add(e.State);
      }
    }

    private void CancelOnCreated(object sender, ResourceReaperStateEventArgs e)
    {
      if (this.id.Equals(e.Instance.SessionId) && ResourceReaperState.Created.Equals(e.State))
      {
        this.cts.Cancel();
      }
    }

    private void CancelOnInitializingConnection(object sender, ResourceReaperStateEventArgs e)
    {
      if (this.id.Equals(e.Instance.SessionId) && ResourceReaperState.InitializingConnection.Equals(e.State))
      {
        this.cts.Cancel();
      }
    }
  }
}
