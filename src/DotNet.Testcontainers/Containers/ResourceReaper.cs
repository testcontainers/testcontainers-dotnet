namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.IO;
  using System.Net.Sockets;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// https://hub.docker.com/r/testcontainers/ryuk.
  /// </summary>
  public sealed class ResourceReaper : IAsyncDisposable
  {
    public const string ResourceReaperSessionLabel = TestcontainersClient.TestcontainersLabel + ".resource-reaper-session";

    private const string RyukImage = "ghcr.io/psanetra/ryuk:2021.12.20";

    private const ushort RyukPort = 8080;

    private static readonly SemaphoreSlim DefaultLock = new SemaphoreSlim(1, 1);

    private static ResourceReaper defaultInstance;

    private readonly ResourceReaperDiagnostics diagnostics = new ResourceReaperDiagnostics();

    private readonly CancellationTokenSource maintainConnectionCts = new CancellationTokenSource();

    private readonly TestcontainersContainer resourceReaperContainer;

    private Task maintainConnectionTask = Task.CompletedTask;

    private bool disposed;

    private ResourceReaper(Guid sessionId, string ryukImage, string dockerEndpoint)
    {
      var builder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithName($"testcontainers-ryuk-{sessionId:D}")
        .WithImage(ryukImage)
        .WithAutoRemove(true)
        .WithCleanUp(false)
        .WithExposedPort(RyukPort)
        .WithPortBinding(RyukPort, true)
        .WithBindMount("/var/run/docker.sock", "/var/run/docker.sock", AccessMode.ReadOnly);

      if (dockerEndpoint != null)
      {
        builder = builder.WithDockerEndpoint(dockerEndpoint);
      }

      this.resourceReaperContainer = builder.Build();
      this.SessionId = sessionId;
    }

    /// <summary>
    /// Occurs when a ResourceReaper state has changed.
    /// </summary>
    /// <remarks>
    /// It emits state changes to uninitialized instances too.
    /// </remarks>
    [PublicAPI]
    public static event EventHandler<ResourceReaperStateEventArgs> StateChanged;

    /// <inheritdoc cref="TestcontainersContainer.Hostname" />
    [NotNull]
    [PublicAPI]
    public string Hostname
    {
      get
      {
        return this.resourceReaperContainer.Hostname;
      }
    }

    /// <summary>
    /// Gets the default <see cref="ResourceReaper" /> session id.
    /// </summary>
    /// <remarks>
    /// The default <see cref="ResourceReaper" /> will start either on <see cref="GetAndStartDefaultAsync(CancellationToken)" />
    /// or if a <see cref="ITestcontainersContainer" /> is configured with <see cref="ITestcontainersBuilder{TDockerContainer}.WithCleanUp" />.
    /// </remarks>
    [PublicAPI]
    public static Guid DefaultSessionId { get; }
      = Guid.NewGuid();

    /// <summary>
    /// Gets the <see cref="ResourceReaper" /> session id.
    /// </summary>
    [PublicAPI]
    public Guid SessionId { get; }

    /// <summary>
    /// Starts and returns the default <see cref="ResourceReaper" /> instance.
    /// </summary>
    /// <param name="dockerEndpoint">The Docker API endpoint.</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization.</param>
    /// <returns>Task that completes when the <see cref="ResourceReaper" /> has been started.</returns>
    [PublicAPI]
    public static async Task<ResourceReaper> GetAndStartDefaultAsync(string dockerEndpoint = null, CancellationToken ct = default)
    {
      if (defaultInstance != null && !defaultInstance.disposed)
      {
        return defaultInstance;
      }

      await DefaultLock.WaitAsync(ct)
        .ConfigureAwait(false);

      if (defaultInstance != null && !defaultInstance.disposed)
      {
        DefaultLock.Release();
        return defaultInstance;
      }

      try
      {
        defaultInstance = await GetAndStartNewAsync(DefaultSessionId, RyukImage, default, dockerEndpoint, ct)
          .ConfigureAwait(false);

        return defaultInstance;
      }
      finally
      {
        DefaultLock.Release();
      }
    }

    /// <summary>
    /// Starts and returns a new <see cref="ResourceReaper" /> instance.
    /// </summary>
    /// <param name="ryukImage">The Ryuk image.</param>
    /// <param name="initTimeout">The timeout to initialize the Ryuk connection (Default: 10 seconds).</param>
    /// <param name="dockerEndpoint">The Docker API endpoint.</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization.</param>
    /// <returns>Task that completes when the <see cref="ResourceReaper" /> has been started.</returns>
    [PublicAPI]
    public static Task<ResourceReaper> GetAndStartNewAsync(string ryukImage = RyukImage, TimeSpan initTimeout = default, string dockerEndpoint = null, CancellationToken ct = default)
    {
      return GetAndStartNewAsync(Guid.NewGuid(), ryukImage, initTimeout, dockerEndpoint, ct);
    }

    /// <summary>
    /// Starts and returns a new <see cref="ResourceReaper" /> instance.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <param name="ryukImage">The Ryuk image.</param>
    /// <param name="initTimeout">The timeout to initialize the Ryuk connection (Default: 10 seconds).</param>
    /// <param name="dockerEndpoint">The Docker API endpoint.</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization.</param>
    /// <returns>Task that completes when the <see cref="ResourceReaper" /> has been started.</returns>
    [PublicAPI]
    public static async Task<ResourceReaper> GetAndStartNewAsync(Guid sessionId, string ryukImage = RyukImage, TimeSpan initTimeout = default, string dockerEndpoint = null, CancellationToken ct = default)
    {
      var ryukInitializedTaskSource = new TaskCompletionSource<bool>();

      var resourceReaper = new ResourceReaper(sessionId, ryukImage, dockerEndpoint);

      initTimeout = TimeSpan.Equals(default, initTimeout) ? TimeSpan.FromSeconds(10) : initTimeout;

      try
      {
        StateChanged?.Invoke(null, new ResourceReaperStateEventArgs(resourceReaper, ResourceReaperState.Created));

        await resourceReaper.resourceReaperContainer.StartAsync(ct)
          .ConfigureAwait(false);

        resourceReaper.diagnostics.ContainerInspection = await resourceReaper.resourceReaperContainer.InspectContainer(ct)
          .ConfigureAwait(false);

        StateChanged?.Invoke(null, new ResourceReaperStateEventArgs(resourceReaper, ResourceReaperState.InitializingConnection));

        using (var initTimeoutCts = new CancellationTokenSource())
        {
          using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(initTimeoutCts.Token, ct))
          {
            resourceReaper.maintainConnectionTask = resourceReaper.MaintainRyukConnection(ryukInitializedTaskSource, linkedCts.Token);
            initTimeoutCts.CancelAfter(initTimeout);

            await ryukInitializedTaskSource.Task
              .ConfigureAwait(false);
          }
        }

        StateChanged?.Invoke(null, new ResourceReaperStateEventArgs(resourceReaper, ResourceReaperState.MaintainingConnection));
      }
      catch (Exception)
      {
        await resourceReaper.DisposeAsync()
          .ConfigureAwait(false);
        throw;
      }

      return resourceReaper;
    }

    /// <inheritdoc />
    [PublicAPI]
    public async ValueTask DisposeAsync()
    {
      if (this.disposed)
      {
        return;
      }

      this.disposed = true;

      this.maintainConnectionCts.Cancel();

      try
      {
        // Close connection before disposing ResourceReaper.
        await this.maintainConnectionTask
          .ConfigureAwait(false);
      }
      catch (Exception)
      {
        // Ignore
      }

      this.maintainConnectionCts.Dispose();

      if (this.resourceReaperContainer != null)
      {
        await this.resourceReaperContainer.DisposeAsync()
          .ConfigureAwait(false);
      }
    }

    private bool TryGetEndpoint(out string host, out ushort port)
    {
      try
      {
        host = this.resourceReaperContainer.Hostname;
        port = this.resourceReaperContainer.GetMappedPublicPort(RyukPort);
        return true;
      }
      catch (Exception e)
      {
        this.resourceReaperContainer.Logger.CanNotGetResourceReaperEndpoint(this.SessionId, e);
        host = null;
        port = 0;
        return false;
      }
    }

    /// <summary>
    /// Establishes and maintains the connection to the running Ryuk container.
    ///
    /// This is split into two phases:
    ///
    /// 1) Initialization
    /// Tries to establish a connection to Ryuk. After establishment, sends a Docker resource filter to Ryuk. Ryuk will acknowledge the reception of the filter.
    /// On termination, Ryuk will delete all Docker resources matching the filter. You can cancel the initialization with <see cref="ct" />.
    ///
    /// 2) Maintenance
    /// After initialization, we need to keep the connection to Ryuk open. If we lose the connection for any reason, Ryuk allows reconnecting within 10 seconds.
    /// It's not necessary to send the filter again after reconnecting.
    /// </summary>
    /// <param name="ryukInitializedTaskSource">The task that completes after the initialization.</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization. This will not cancel the maintained connection.</param>
    private async Task MaintainRyukConnection(TaskCompletionSource<bool> ryukInitializedTaskSource, CancellationToken ct)
    {
      while (!this.maintainConnectionCts.IsCancellationRequested && (!ct.IsCancellationRequested || ryukInitializedTaskSource.Task.IsCompleted))
      {
        if (!this.TryGetEndpoint(out var host, out var port))
        {
          await Task.Delay(TimeSpan.FromSeconds(1), default)
            .ConfigureAwait(false);

          continue;
        }

        this.diagnostics.ExpectedHost = host;
        this.diagnostics.ExpectedPort = port;
        this.diagnostics.IncrementConnectionAttempts();

        using (var tcpClient = new TcpClient())
        {
          try
          {
            await tcpClient.ConnectAsync(host, port)
              .ConfigureAwait(false);

            var stream = tcpClient.GetStream();

            var filter = $"label={ResourceReaperSessionLabel}={this.SessionId:D}\n";

            var sendBytes = Encoding.ASCII.GetBytes(filter);

            var readBytes = new byte[tcpClient.ReceiveBufferSize];

            if (!ryukInitializedTaskSource.Task.IsCompleted)
            {
              using (var messageBuffer = new MemoryStream())
              {
#if NETSTANDARD2_1_OR_GREATER
                await stream.WriteAsync(new ReadOnlyMemory<byte>(sendBytes), ct)
                  .ConfigureAwait(false);
#else
                await stream.WriteAsync(sendBytes, 0, sendBytes.Length, ct)
                  .ConfigureAwait(false);
#endif

                await stream.FlushAsync(ct)
                  .ConfigureAwait(false);

                bool hasAcknowledge;

                do
                {
#if NETSTANDARD2_1_OR_GREATER
                  var numberOfBytes = await stream.ReadAsync(new Memory<byte>(readBytes), ct)
                    .ConfigureAwait(false);
#else
                  var numberOfBytes = await stream.ReadAsync(readBytes, 0, readBytes.Length, ct)
                    .ConfigureAwait(false);
#endif

                  var indexOfNewLine = Array.IndexOf(readBytes, (byte)'\n');

                  if (indexOfNewLine == -1)
                  {
                    // We have not received the entire message yet. Read from stream again.
                    messageBuffer.Write(readBytes, 0, numberOfBytes);
                    hasAcknowledge = false;
                  }
                  else
                  {
                    messageBuffer.Write(readBytes, 0, indexOfNewLine);
                    hasAcknowledge = "ack".Equals(Encoding.ASCII.GetString(messageBuffer.ToArray()), StringComparison.OrdinalIgnoreCase);
                    messageBuffer.SetLength(0);
                  }
                }
                while (!hasAcknowledge);

                ryukInitializedTaskSource.SetResult(true);
              }
            }

            while (!this.maintainConnectionCts.IsCancellationRequested)
            {
              // Keep the connection to Ryuk up.
#if NETSTANDARD2_1_OR_GREATER
              _ = await stream.ReadAsync(new Memory<byte>(readBytes), this.maintainConnectionCts.Token)
                .ConfigureAwait(false);
#else
              _ = await stream.ReadAsync(readBytes, 0, readBytes.Length, this.maintainConnectionCts.Token)
                .ConfigureAwait(false);
#endif
            }
          }
          catch (OperationCanceledException)
          {
            // Ignore cancellation.
          }
          catch (SocketException e)
          {
            this.resourceReaperContainer.Logger.CanNotConnectToResourceReaper(this.SessionId, host, port, e);

            await Task.Delay(TimeSpan.FromSeconds(1), default)
              .ConfigureAwait(false);
          }
          catch (Exception e)
          {
            this.resourceReaperContainer.Logger.LostConnectionToResourceReaper(this.SessionId, host, port, e);

            await Task.Delay(TimeSpan.FromSeconds(1), default)
              .ConfigureAwait(false);
          }
        }
      }

      if (ryukInitializedTaskSource.Task.IsCompleted)
      {
        StateChanged?.Invoke(null, new ResourceReaperStateEventArgs(this, ResourceReaperState.ConnectionTerminated));
        return;
      }

      if (ct.IsCancellationRequested)
      {
        ryukInitializedTaskSource.SetException(new ResourceReaperException("Initialization has been cancelled.", this.diagnostics));
      }
      else
      {
        ryukInitializedTaskSource.SetException(new ResourceReaperException("Initialization failed.", this.diagnostics));
      }
    }
  }
}
