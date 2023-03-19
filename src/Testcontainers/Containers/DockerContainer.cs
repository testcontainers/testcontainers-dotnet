namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IContainer" />
  [PublicAPI]
  public class DockerContainer : Resource, IContainer
  {
    private const TestcontainersStates ContainerHasBeenCreatedStates = TestcontainersStates.Created | TestcontainersStates.Running | TestcontainersStates.Exited;

    private const TestcontainersHealthStatus ContainerHasHealthCheck = TestcontainersHealthStatus.Starting | TestcontainersHealthStatus.Healthy | TestcontainersHealthStatus.Unhealthy;

    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly ITestcontainersClient client;

    private readonly IContainerConfiguration configuration;

    private ContainerInspectResponse container = new ContainerInspectResponse();

    private int disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerContainer(IContainerConfiguration configuration, ILogger logger)
    {
      this.client = new TestcontainersClient(configuration.SessionId, configuration.DockerEndpointAuthConfig, logger);
      this.configuration = configuration;
      this.Logger = logger;
    }

    /// <inheritdoc />
    public event EventHandler Creating;

    /// <inheritdoc />
    public event EventHandler Starting;

    /// <inheritdoc />
    public event EventHandler Stopping;

    /// <inheritdoc />
    public event EventHandler Created;

    /// <inheritdoc />
    public event EventHandler Started;

    /// <inheritdoc />
    public event EventHandler Stopped;

    /// <inheritdoc />
    public ILogger Logger { get; }

    /// <inheritdoc />
    public string Id
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.container.ID;
      }
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.container.Name;
      }
    }

    /// <inheritdoc />
    public string IpAddress
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.container.NetworkSettings.Networks.First().Value.IPAddress;
      }
    }

    /// <inheritdoc />
    public string MacAddress
    {
      get
      {
        this.ThrowIfResourceNotFound();
        return this.container.NetworkSettings.Networks.First().Value.MacAddress;
      }
    }

    /// <inheritdoc />
    public string Hostname
    {
      get
      {
        if (!string.IsNullOrEmpty(TestcontainersSettings.DockerHostOverride))
        {
          return TestcontainersSettings.DockerHostOverride;
        }

        var dockerEndpoint = this.configuration.DockerEndpointAuthConfig.Endpoint;

        switch (dockerEndpoint.Scheme)
        {
          case "http":
          case "https":
          case "tcp":
          {
            return dockerEndpoint.Host;
          }

          case "npipe":
          case "unix":
          {
            const string localhost = "127.0.0.1";

            if (!this.Exists())
            {
              return localhost;
            }

            if (!this.client.IsRunningInsideDocker)
            {
              return localhost;
            }

            var endpointSettings = this.container.NetworkSettings.Networks.First().Value;
            return endpointSettings.Gateway;
          }

          default:
            throw new InvalidOperationException($"Docker endpoint {dockerEndpoint} is not supported.");
        }
      }
    }

    /// <inheritdoc />
    public IImage Image
    {
      get
      {
        return this.configuration.Image;
      }
    }

    /// <inheritdoc />
    public TestcontainersStates State
    {
      get
      {
        if (this.container.State == null)
        {
          return TestcontainersStates.Undefined;
        }

        try
        {
          return (TestcontainersStates)Enum.Parse(typeof(TestcontainersStates), this.container.State.Status, true);
        }
        catch (Exception)
        {
          return TestcontainersStates.Undefined;
        }
      }
    }

    /// <inheritdoc />
    public TestcontainersHealthStatus Health
    {
      get
      {
        if (this.container.State == null)
        {
          return TestcontainersHealthStatus.Undefined;
        }

        if (this.container.State.Health == null)
        {
          return TestcontainersHealthStatus.None;
        }

        try
        {
          return (TestcontainersHealthStatus)Enum.Parse(typeof(TestcontainersHealthStatus), this.container.State.Health.Status, true);
        }
        catch (Exception)
        {
          return TestcontainersHealthStatus.Undefined;
        }
      }
    }

    /// <inheritdoc />
    public long HealthCheckFailingStreak
    {
      get
      {
        return ContainerHasHealthCheck.HasFlag(this.Health) ? this.container.State.Health.FailingStreak : 0;
      }
    }

    /// <inheritdoc />
    public ushort GetMappedPublicPort(int containerPort)
    {
      return this.GetMappedPublicPort(Convert.ToString(containerPort, CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public ushort GetMappedPublicPort(string containerPort)
    {
      this.ThrowIfResourceNotFound();

      if (this.container.NetworkSettings.Ports.TryGetValue($"{containerPort}/tcp", out var portBindings) && ushort.TryParse(portBindings.First().HostPort, out var publicPort))
      {
        return publicPort;
      }
      else
      {
        throw new InvalidOperationException($"Exposed port {containerPort} is not mapped.");
      }
    }

    /// <inheritdoc />
    public Task<long> GetExitCode(CancellationToken ct = default)
    {
      return this.GetExitCodeAsync(ct);
    }

    /// <inheritdoc />
    public Task<long> GetExitCodeAsync(CancellationToken ct = default)
    {
      return this.client.GetContainerExitCodeAsync(this.Id, ct);
    }

    /// <inheritdoc />
    public Task<(string Stdout, string Stderr)> GetLogs(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default)
    {
      return this.GetLogsAsync(since, until, timestampsEnabled, ct);
    }

    /// <inheritdoc />
    public Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default)
    {
      return this.client.GetContainerLogsAsync(this.Id, since, until, timestampsEnabled, ct);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
      await this.DisposeAsyncCore()
        .ConfigureAwait(false);

      GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual async Task StartAsync(CancellationToken ct = default)
    {
      using (_ = new AcquireLock(this.semaphoreSlim))
      {
        await this.UnsafeCreateAsync(ct)
          .ConfigureAwait(false);

        await this.UnsafeStartAsync(ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken ct = default)
    {
      using (_ = new AcquireLock(this.semaphoreSlim))
      {
        await this.UnsafeStopAsync(ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public Task CopyFileAsync(string filePath, byte[] fileContent, int accessMode = 384, int userId = 0, int groupId = 0, CancellationToken ct = default)
    {
      return this.client.CopyFileAsync(this.Id, filePath, fileContent, accessMode, userId, groupId, ct);
    }

    /// <inheritdoc />
    public Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default)
    {
      return this.client.ReadFileAsync(this.Id, filePath, ct);
    }

    /// <inheritdoc />
    public Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default)
    {
      return this.client.ExecAsync(this.Id, command, ct);
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
    protected virtual async ValueTask DisposeAsyncCore()
    {
      if (1.Equals(Interlocked.CompareExchange(ref this.disposed, 1, 0)))
      {
        return;
      }

      using (_ = new AcquireLock(this.semaphoreSlim))
      {
        if (Guid.Empty.Equals(this.configuration.SessionId))
        {
          await this.UnsafeStopAsync()
            .ConfigureAwait(false);
        }
        else
        {
          await this.UnsafeDeleteAsync()
            .ConfigureAwait(false);
        }
      }

      this.semaphoreSlim.Dispose();
    }

    /// <summary>
    /// Creates the container.
    /// </summary>
    /// <remarks>
    /// Only the public members <see cref="StartAsync" /> and <see cref="StopAsync" /> are thread-safe for now.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been created.</returns>
    protected virtual async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
      this.ThrowIfLockNotAcquired();

      if (this.Exists())
      {
        return;
      }

      this.Creating?.Invoke(this, EventArgs.Empty);

      var id = await this.client.RunAsync(this.configuration, ct)
        .ConfigureAwait(false);

      this.container = await this.client.InspectContainerAsync(id, ct)
        .ConfigureAwait(false);

      this.Created?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Deletes the container.
    /// </summary>
    /// <remarks>
    /// Only the public members <see cref="StartAsync" /> and <see cref="StopAsync" /> are thread-safe for now.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been deleted.</returns>
    protected virtual async Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
      this.ThrowIfLockNotAcquired();

      if (!this.Exists())
      {
        return;
      }

      await this.client.RemoveAsync(this.container.ID, ct)
        .ConfigureAwait(false);

      this.container = new ContainerInspectResponse();
    }

    /// <summary>
    /// Starts the container.
    /// </summary>
    /// <remarks>
    /// Only the public members <see cref="StartAsync" /> and <see cref="StopAsync" /> are thread-safe for now.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been started.</returns>
    protected virtual async Task UnsafeStartAsync(CancellationToken ct = default)
    {
      this.ThrowIfLockNotAcquired();

      async Task<bool> CheckPortBindings()
      {
        this.container = await this.client.InspectContainerAsync(this.container.ID, ct)
          .ConfigureAwait(false);

        var boundPorts = this.container.NetworkSettings.Ports.Values.Where(portBindings => portBindings != null).SelectMany(portBinding => portBinding).Count(portBinding => !string.IsNullOrEmpty(portBinding.HostPort));
        return this.configuration.PortBindings == null || /* IPv4 or IPv6 */ this.configuration.PortBindings.Count == boundPorts || /* IPv4 and IPv6 */ 2 * this.configuration.PortBindings.Count == boundPorts;
      }

      async Task<bool> CheckWaitStrategy(IWaitUntil wait)
      {
        this.container = await this.client.InspectContainerAsync(this.container.ID, ct)
          .ConfigureAwait(false);

        return await wait.UntilAsync(this)
          .ConfigureAwait(false);
      }

      await this.client.StartAsync(this.container.ID, ct)
        .ConfigureAwait(false);

      await WaitStrategy.WaitUntilAsync(CheckPortBindings, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15), ct)
        .ConfigureAwait(false);

      this.Starting?.Invoke(this, EventArgs.Empty);

      await this.configuration.StartupCallback(this, ct)
        .ConfigureAwait(false);

      foreach (var waitStrategy in this.configuration.WaitStrategies)
      {
        await WaitStrategy.WaitUntilAsync(() => CheckWaitStrategy(waitStrategy), TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan, ct)
          .ConfigureAwait(false);
      }

      this.Started?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Stops the container.
    /// </summary>
    /// <remarks>
    /// Only the public members <see cref="StartAsync" /> and <see cref="StopAsync" /> are thread-safe for now.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been stopped.</returns>
    protected virtual async Task UnsafeStopAsync(CancellationToken ct = default)
    {
      this.ThrowIfLockNotAcquired();

      if (!this.Exists())
      {
        return;
      }

      this.Stopping?.Invoke(this, EventArgs.Empty);

      await this.client.StopAsync(this.container.ID, ct)
        .ConfigureAwait(false);

      try
      {
        this.container = await this.client.InspectContainerAsync(this.container.ID, ct)
          .ConfigureAwait(false);
      }
      catch (DockerContainerNotFoundException)
      {
        this.container = new ContainerInspectResponse();
      }

      this.Stopped?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override bool Exists()
    {
      return ContainerHasBeenCreatedStates.HasFlag(this.State);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException" /> when the lock is not acquired.
    /// </summary>
    /// <exception cref="InvalidOperationException">The lock is not acquired.</exception>
    protected virtual void ThrowIfLockNotAcquired()
    {
      _ = Guard.Argument(this.semaphoreSlim, nameof(this.semaphoreSlim))
        .ThrowIf(argument => argument.Value.CurrentCount > 0, _ => new InvalidOperationException("Unsafe method call requires lock."));
    }

    private sealed class AcquireLock : IDisposable
    {
      private readonly SemaphoreSlim semaphoreSlim;

      public AcquireLock(SemaphoreSlim semaphoreSlim)
      {
        this.semaphoreSlim = semaphoreSlim;
        this.semaphoreSlim.Wait();
      }

      public void Dispose()
      {
        this.semaphoreSlim.Release();
      }
    }
  }
}
