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
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// The Resource Reaper takes care of the remaining Docker resources and removes them: https://dotnet.testcontainers.org/api/resource-reaper/.
  /// </summary>
  [PublicAPI]
  public sealed class ResourceReaper : IAsyncDisposable
  {
    public const string ResourceReaperSessionLabel = TestcontainersClient.TestcontainersLabel + ".resource-reaper-session";

    private const ushort RyukPort = 8080;

    /// <summary>
    /// 60 seconds connection timeout.
    /// </summary>
    private const int ConnectionTimeoutInSeconds = 60;

    /// <summary>
    /// 2 seconds retry timeout.
    /// </summary>
    private const int RetryTimeoutInSeconds = 2;

    private static readonly IImage RyukImage = new DockerImage("testcontainers/ryuk:0.6.0");

    private static readonly SemaphoreSlim DefaultLock = new SemaphoreSlim(1, 1);

    private static readonly LingerOption DiscardAllPendingData = new LingerOption(true, 0);

    private static ResourceReaper _defaultInstance;

    private readonly CancellationTokenSource _maintainConnectionCts = new CancellationTokenSource();

    private readonly IContainer _resourceReaperContainer;

    private Task _maintainConnectionTask = Task.CompletedTask;

    private bool _disposed;

    static ResourceReaper()
    {
    }

    private ResourceReaper(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, IImage resourceReaperImage, IMount dockerSocket, bool requiresPrivilegedMode)
    {
      _resourceReaperContainer = new ContainerBuilder()
        .WithName($"testcontainers-ryuk-{sessionId:D}")
        .WithDockerEndpoint(dockerEndpointAuthConfig)
        .WithImage(resourceReaperImage)
        .WithPrivileged(requiresPrivilegedMode)
        .WithAutoRemove(true)
        .WithCleanUp(false)
        .WithPortBinding(TestcontainersSettings.ResourceReaperPublicHostPort.Invoke(dockerEndpointAuthConfig), RyukPort)
        .WithMount(dockerSocket)
        .Build();

      SessionId = sessionId;
    }

    /// <summary>
    /// Occurs when a Resource Reaper state has changed.
    /// </summary>
    /// <remarks>
    /// It emits state changes to uninitialized instances too.
    /// </remarks>
    [PublicAPI]
    public static event EventHandler<ResourceReaperStateEventArgs> StateChanged;

    /// <summary>
    /// Gets the default <see cref="ResourceReaper" /> session id.
    /// </summary>
    /// <remarks>
    /// The default <see cref="ResourceReaper" /> will start either on <see cref="GetAndStartDefaultAsync(IDockerEndpointAuthenticationConfiguration, bool, CancellationToken)" />
    /// or if a <see cref="IContainer" /> is configured with <see cref="IAbstractBuilder{TBuilderEntity, TContainerEntity, TCreateResourceEntity}.WithCleanUp" />.
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
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    /// <param name="isWindowsEngineEnabled">Determines whether the Windows engine is enabled or not.</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization.</param>
    /// <returns>Task that completes when the <see cref="ResourceReaper" /> has been started.</returns>
    [PublicAPI]
    public static async Task<ResourceReaper> GetAndStartDefaultAsync(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, bool isWindowsEngineEnabled = false, CancellationToken ct = default)
    {
      if (isWindowsEngineEnabled)
      {
        return null;
      }

      if (_defaultInstance != null && !_defaultInstance._disposed)
      {
        return _defaultInstance;
      }

      await DefaultLock.WaitAsync(ct)
        .ConfigureAwait(false);

      if (_defaultInstance != null && !_defaultInstance._disposed)
      {
        DefaultLock.Release();
        return _defaultInstance;
      }

      try
      {
        var resourceReaperImage = TestcontainersSettings.ResourceReaperImage ?? RyukImage;

        var requiresPrivilegedMode = TestcontainersSettings.ResourceReaperPrivilegedModeEnabled;

        _defaultInstance = await GetAndStartNewAsync(DefaultSessionId, dockerEndpointAuthConfig, resourceReaperImage, new UnixSocketMount(dockerEndpointAuthConfig.Endpoint), requiresPrivilegedMode, ct: ct)
          .ConfigureAwait(false);

        return _defaultInstance;
      }
      finally
      {
        DefaultLock.Release();
      }
    }

    /// <inheritdoc />
    [PublicAPI]
    public async ValueTask DisposeAsync()
    {
      if (_disposed)
      {
        return;
      }

      _disposed = true;

      try
      {
        _maintainConnectionCts.Cancel();

        // Close connection before disposing Resource Reaper.
        await _maintainConnectionTask
          .ConfigureAwait(false);
      }
      finally
      {
        _maintainConnectionCts.Dispose();
      }

      if (_resourceReaperContainer != null)
      {
        await _resourceReaperContainer.DisposeAsync()
          .ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Starts and returns a new <see cref="ResourceReaper" /> instance.
    /// </summary>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    /// <param name="resourceReaperImage">The Resource Reaper image.</param>
    /// <param name="dockerSocket">The Docker socket.</param>
    /// <param name="requiresPrivilegedMode">True if the container requires privileged mode, otherwise false.</param>
    /// <param name="initTimeout">The timeout to initialize the Ryuk connection (Default: <inheritdoc cref="ConnectionTimeoutInSeconds" />).</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization.</param>
    /// <returns>Task that completes when the <see cref="ResourceReaper" /> has been started.</returns>
    [PublicAPI]
    private static Task<ResourceReaper> GetAndStartNewAsync(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, IImage resourceReaperImage, IMount dockerSocket, bool requiresPrivilegedMode = false, TimeSpan initTimeout = default, CancellationToken ct = default)
    {
      return GetAndStartNewAsync(Guid.NewGuid(), dockerEndpointAuthConfig, resourceReaperImage, dockerSocket, requiresPrivilegedMode, initTimeout, ct);
    }

    /// <summary>
    /// Starts and returns a new <see cref="ResourceReaper" /> instance.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    /// <param name="resourceReaperImage">The Resource Reaper image.</param>
    /// <param name="dockerSocket">The Docker socket.</param>
    /// <param name="requiresPrivilegedMode">True if the container requires privileged mode, otherwise false.</param>
    /// <param name="initTimeout">The timeout to initialize the Ryuk connection (Default: <inheritdoc cref="ConnectionTimeoutInSeconds" />).</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization.</param>
    /// <returns>Task that completes when the <see cref="ResourceReaper" /> has been started.</returns>
    [PublicAPI]
    private static async Task<ResourceReaper> GetAndStartNewAsync(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, IImage resourceReaperImage, IMount dockerSocket, bool requiresPrivilegedMode = false, TimeSpan initTimeout = default, CancellationToken ct = default)
    {
      var ryukInitializedTaskSource = new TaskCompletionSource<bool>();

      var resourceReaper = new ResourceReaper(sessionId, dockerEndpointAuthConfig, resourceReaperImage, dockerSocket, requiresPrivilegedMode);

      initTimeout = TimeSpan.Equals(default, initTimeout) ? TimeSpan.FromSeconds(ConnectionTimeoutInSeconds) : initTimeout;

      try
      {
        StateChanged?.Invoke(null, new ResourceReaperStateEventArgs(resourceReaper, ResourceReaperState.Created));

        await resourceReaper._resourceReaperContainer.StartAsync(ct)
          .ConfigureAwait(false);

        StateChanged?.Invoke(null, new ResourceReaperStateEventArgs(resourceReaper, ResourceReaperState.InitializingConnection));

        using (var initTimeoutCts = new CancellationTokenSource())
        {
          using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(initTimeoutCts.Token, ct))
          {
            resourceReaper._maintainConnectionTask = resourceReaper.MaintainRyukConnection(ryukInitializedTaskSource, linkedCts.Token);
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

    private bool TryGetEndpoint(out string host, out ushort port)
    {
      try
      {
        host = _resourceReaperContainer.Hostname;
        port = _resourceReaperContainer.GetMappedPublicPort(RyukPort);
        return true;
      }
      catch (Exception e)
      {
        _resourceReaperContainer.Logger.CanNotGetResourceReaperEndpoint(SessionId, e);
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
    /// On termination, Ryuk will delete all Docker resources matching the filter. You can cancel the initialization with <paramref name="ct" />.
    ///
    /// 2) Maintenance
    /// After initialization, we need to keep the connection to Ryuk open. If we lose the connection for any reason, Ryuk allows reconnecting within 10 seconds.
    /// It's not necessary to send the filter again after reconnecting.
    /// </summary>
    /// <param name="ryukInitializedTaskSource">The task that completes after the initialization.</param>
    /// <param name="ct">The cancellation token to cancel the <see cref="ResourceReaper" /> initialization. This will not cancel the maintained connection.</param>
    private async Task MaintainRyukConnection(TaskCompletionSource<bool> ryukInitializedTaskSource, CancellationToken ct)
    {
      connect_to_ryuk: while (!_maintainConnectionCts.IsCancellationRequested && !ct.IsCancellationRequested && !ryukInitializedTaskSource.Task.IsCompleted)
      {
        if (!TryGetEndpoint(out var host, out var port))
        {
          await Task.Delay(TimeSpan.FromSeconds(RetryTimeoutInSeconds), CancellationToken.None)
            .ConfigureAwait(false);

          continue;
        }

        using (var tcpClient = new TcpClient())
        {
          tcpClient.LingerState = DiscardAllPendingData;

          try
          {
#if NET6_0_OR_GREATER
            await tcpClient.ConnectAsync(host, port, ct)
              .ConfigureAwait(false);
#else
            await tcpClient.ConnectAsync(host, port)
              .ConfigureAwait(false);
#endif

            var stream = tcpClient.GetStream();

            var filter = $"label={ResourceReaperSessionLabel}={SessionId:D}\n";

            var sendBytes = Encoding.ASCII.GetBytes(filter);

            var readBytes = new byte[tcpClient.ReceiveBufferSize];

            if (!ryukInitializedTaskSource.Task.IsCompleted)
            {
              using (var messageBuffer = new MemoryStream())
              {
#if NETSTANDARD2_0
                await stream.WriteAsync(sendBytes, 0, sendBytes.Length, ct)
                  .ConfigureAwait(false);
#else
                await stream.WriteAsync(sendBytes, ct)
                  .ConfigureAwait(false);
#endif

                await stream.FlushAsync(ct)
                  .ConfigureAwait(false);

                bool hasAcknowledge;

                do
                {
#if NETSTANDARD2_0
                  var numberOfBytes = await stream.ReadAsync(readBytes, 0, readBytes.Length, ct)
                    .ConfigureAwait(false);
#else
                  var numberOfBytes = await stream.ReadAsync(readBytes, ct)
                    .ConfigureAwait(false);
#endif

                  if (numberOfBytes == 0)
                  {
                    // Even if there is no listening socket behind the bound port, the TcpClient establishes a connection.
                    // If we do not receive any data, the socket is not ready yet.
                    await Task.Delay(TimeSpan.FromSeconds(RetryTimeoutInSeconds), ct)
                      .ConfigureAwait(false);

#pragma warning disable S907

                    goto connect_to_ryuk;

#pragma warning restore S907
                  }

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

            while (!_maintainConnectionCts.IsCancellationRequested)
            {
              // Keep the connection to Ryuk up.
#if NETSTANDARD2_0
              _ = await stream.ReadAsync(readBytes, 0, readBytes.Length, _maintainConnectionCts.Token)
                .ConfigureAwait(false);
#else
              _ = await stream.ReadAsync(readBytes, _maintainConnectionCts.Token)
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
            _resourceReaperContainer.Logger.CanNotConnectToResourceReaper(SessionId, host, port, e);

            await Task.Delay(TimeSpan.FromSeconds(RetryTimeoutInSeconds), CancellationToken.None)
              .ConfigureAwait(false);
          }
          catch (Exception e)
          {
            _resourceReaperContainer.Logger.LostConnectionToResourceReaper(SessionId, host, port, e);

            await Task.Delay(TimeSpan.FromSeconds(RetryTimeoutInSeconds), CancellationToken.None)
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
        ryukInitializedTaskSource.SetException(new ResourceReaperException("Initialization has been cancelled."));
      }
      else
      {
        ryukInitializedTaskSource.SetException(new ResourceReaperException("Initialization failed."));
      }
    }

    private sealed class UnixSocketMount : IMount
    {
      private const string DockerSocketFilePath = "/var/run/docker.sock";

      public UnixSocketMount([NotNull] Uri dockerEndpoint)
      {
        // If the Docker endpoint is a Unix socket, extract the socket path from the URI; otherwise, fallback to the default Unix socket path.
        Source = "unix".Equals(dockerEndpoint.Scheme, StringComparison.OrdinalIgnoreCase) ? dockerEndpoint.AbsolutePath : DockerSocketFilePath;

        // If the user has overridden the Docker socket path, use the user-specified path; otherwise, keep the previously determined source.
        Source = !string.IsNullOrEmpty(TestcontainersSettings.DockerSocketOverride) ? TestcontainersSettings.DockerSocketOverride : Source;
        Target = DockerSocketFilePath;
      }

      public MountType Type
        => MountType.Bind;

      public AccessMode AccessMode
        => AccessMode.ReadOnly;

      public string Source { get; }

      public string Target { get; }

      public Task CreateAsync(CancellationToken ct = default)
      {
        return Task.CompletedTask;
      }

      public Task DeleteAsync(CancellationToken ct = default)
      {
        return Task.CompletedTask;
      }
    }
  }
}
