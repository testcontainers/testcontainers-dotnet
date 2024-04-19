namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
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

    private readonly ITestcontainersClient _client;

    private readonly IContainerConfiguration _configuration;

    private ContainerInspectResponse _container = new ContainerInspectResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerContainer(IContainerConfiguration configuration, ILogger logger)
    {
      _client = new TestcontainersClient(configuration.SessionId, configuration.DockerEndpointAuthConfig, logger);
      _configuration = configuration;
      Logger = logger;
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
    public DateTime CreatedTime { get; private set; }

    /// <inheritdoc />
    public DateTime StartedTime { get; private set; }

    /// <inheritdoc />
    public DateTime StoppedTime { get; private set; }

    /// <inheritdoc />
    public string Id
    {
      get
      {
        ThrowIfResourceNotFound();
        return _container.ID;
      }
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        ThrowIfResourceNotFound();
        return _container.Name;
      }
    }

    /// <inheritdoc />
    public string IpAddress
    {
      get
      {
        ThrowIfResourceNotFound();
        return _container.NetworkSettings.Networks.First().Value.IPAddress;
      }
    }

    /// <inheritdoc />
    public string MacAddress
    {
      get
      {
        ThrowIfResourceNotFound();
        return _container.NetworkSettings.Networks.First().Value.MacAddress;
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

        var dockerEndpoint = _configuration.DockerEndpointAuthConfig.Endpoint;

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

            if (!Exists())
            {
              return localhost;
            }

            if (!_client.IsRunningInsideDocker)
            {
              return localhost;
            }

            var endpointSettings = _container.NetworkSettings.Networks.First().Value;
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
        return _configuration.Image;
      }
    }

    /// <inheritdoc />
    public TestcontainersStates State
    {
      get
      {
        if (_container.State == null)
        {
          return TestcontainersStates.Undefined;
        }

        try
        {
          return (TestcontainersStates)Enum.Parse(typeof(TestcontainersStates), _container.State.Status, true);
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
        if (_container.State == null)
        {
          return TestcontainersHealthStatus.Undefined;
        }

        if (_container.State.Health == null)
        {
          return TestcontainersHealthStatus.None;
        }

        try
        {
          return (TestcontainersHealthStatus)Enum.Parse(typeof(TestcontainersHealthStatus), _container.State.Health.Status, true);
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
        return ContainerHasHealthCheck.HasFlag(Health) ? _container.State.Health.FailingStreak : 0;
      }
    }

    /// <inheritdoc />
    public ushort GetMappedPublicPort(int containerPort)
    {
      return GetMappedPublicPort(Convert.ToString(containerPort, CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public ushort GetMappedPublicPort(string containerPort)
    {
      ThrowIfResourceNotFound();

      if (_container.NetworkSettings.Ports.TryGetValue($"{containerPort}/tcp", out var portBindings) && ushort.TryParse(portBindings[0].HostPort, out var publicPort))
      {
        return publicPort;
      }
      else
      {
        throw new InvalidOperationException($"Exposed port {containerPort} is not mapped.");
      }
    }

    /// <inheritdoc />
    public Task<long> GetExitCodeAsync(CancellationToken ct = default)
    {
      return _client.GetContainerExitCodeAsync(Id, ct);
    }

    /// <inheritdoc />
    public Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default)
    {
      return _client.GetContainerLogsAsync(Id, since, until, timestampsEnabled, ct);
    }

    /// <inheritdoc />
    public virtual async Task StartAsync(CancellationToken ct = default)
    {
      using (_ = AcquireLock())
      {
        var futureResources = Array.Empty<IFutureResource>()
          .Concat(_configuration.Mounts)
          .Concat(_configuration.Networks);

        await Task.WhenAll(futureResources.Select(resource => resource.CreateAsync(ct)))
          .ConfigureAwait(false);

        await Task.WhenAll(_configuration.Containers.Select(resource => resource.StartAsync(ct)))
          .ConfigureAwait(false);

        await UnsafeCreateAsync(ct)
          .ConfigureAwait(false);

        await UnsafeStartAsync(ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken ct = default)
    {
      using (_ = AcquireLock())
      {
        await UnsafeStopAsync(ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public Task CopyAsync(byte[] fileContent, string filePath, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default)
    {
      return _client.CopyAsync(Id, new BinaryResourceMapping(fileContent, filePath, fileMode), ct);
    }

    /// <inheritdoc />
    public Task CopyAsync(string source, string target, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default)
    {
      var fileAttributes = File.GetAttributes(source);

      if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
      {
        return CopyAsync(new DirectoryInfo(source), target, fileMode, ct);
      }
      else
      {
        return CopyAsync(new FileInfo(source), target, fileMode, ct);
      }
    }

    /// <inheritdoc />
    public Task CopyAsync(FileInfo source, string target, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default)
    {
      return _client.CopyAsync(Id, source, target, fileMode, ct);
    }

    /// <inheritdoc />
    public Task CopyAsync(DirectoryInfo source, string target, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default)
    {
      return _client.CopyAsync(Id, source, target, fileMode, ct);
    }

    /// <inheritdoc />
    public Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default)
    {
      return _client.ReadFileAsync(Id, filePath, ct);
    }

    /// <inheritdoc />
    public Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default)
    {
      return _client.ExecAsync(Id, command, ct);
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
    protected override async ValueTask DisposeAsyncCore()
    {
      if (Disposed)
      {
        return;
      }

      using (_ = AcquireLock())
      {
        if (Guid.Empty.Equals(_configuration.SessionId))
        {
          await UnsafeStopAsync()
            .ConfigureAwait(false);
        }
        else
        {
          await UnsafeDeleteAsync()
            .ConfigureAwait(false);
        }
      }

      await base.DisposeAsyncCore()
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Only the public members <see cref="StartAsync" /> and <see cref="StopAsync" /> are thread-safe for now.
    /// </remarks>
    protected override async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
      ThrowIfLockNotAcquired();

      if (Exists())
      {
        return;
      }

      Creating?.Invoke(this, EventArgs.Empty);

      string id;

      if (_configuration.Reuse.HasValue && _configuration.Reuse.Value)
      {
        Logger.ReusableExperimentalFeature();

        var filters = new FilterByReuseHash(_configuration);

        var reusableContainers = await _client.Container.GetAllAsync(filters, ct)
          .ConfigureAwait(false);

        var reusableContainer = reusableContainers.SingleOrDefault();

        if (reusableContainer != null)
        {
          Logger.ReusableResourceFound();

          id = reusableContainer.ID;
        }
        else
        {
          Logger.ReusableResourceNotFound();

          id = await _client.RunAsync(_configuration, ct)
            .ConfigureAwait(false);
        }
      }
      else
      {
        id = await _client.RunAsync(_configuration, ct)
          .ConfigureAwait(false);
      }

      _container = await _client.Container.ByIdAsync(id, ct)
        .ConfigureAwait(false);

      CreatedTime = DateTime.UtcNow;
      Created?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Only the public members <see cref="StartAsync" /> and <see cref="StopAsync" /> are thread-safe for now.
    /// </remarks>
    protected override async Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
      ThrowIfLockNotAcquired();

      if (!Exists())
      {
        return;
      }

      await _client.RemoveAsync(_container.ID, ct)
        .ConfigureAwait(false);

      _container = new ContainerInspectResponse();

      CreatedTime = default;
      StartedTime = default;
      StoppedTime = default;
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
      ThrowIfLockNotAcquired();

      async Task<bool> CheckPortBindingsAsync()
      {
        _container = await _client.Container.ByIdAsync(_container.ID, ct)
          .ConfigureAwait(false);

        var boundPorts = _container.NetworkSettings.Ports.Values.Where(portBindings => portBindings != null).SelectMany(portBinding => portBinding).Count(portBinding => !string.IsNullOrEmpty(portBinding.HostPort));
        return _configuration.PortBindings == null || /* IPv4 or IPv6 */ _configuration.PortBindings.Count == boundPorts || /* IPv4 and IPv6 */ 2 * _configuration.PortBindings.Count == boundPorts;
      }

      async Task<bool> CheckWaitStrategyAsync(IWaitUntil wait)
      {
        _container = await _client.Container.ByIdAsync(_container.ID, ct)
          .ConfigureAwait(false);

        return await wait.UntilAsync(this)
          .ConfigureAwait(false);
      }

      await _client.AttachAsync(_container.ID, _configuration.OutputConsumer, ct)
        .ConfigureAwait(false);

      await _client.StartAsync(_container.ID, ct)
        .ConfigureAwait(false);

      await WaitStrategy.WaitUntilAsync(CheckPortBindingsAsync, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15), ct)
        .ConfigureAwait(false);

      Starting?.Invoke(this, EventArgs.Empty);

      await _configuration.StartupCallback(this, ct)
        .ConfigureAwait(false);

      Logger.StartReadinessCheck(_container.ID);

      foreach (var waitStrategy in _configuration.WaitStrategies)
      {
        await WaitStrategy.WaitUntilAsync(() => CheckWaitStrategyAsync(waitStrategy), TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan, ct)
          .ConfigureAwait(false);
      }

      Logger.CompleteReadinessCheck(_container.ID);

      StartedTime = DateTime.UtcNow;
      Started?.Invoke(this, EventArgs.Empty);
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
      ThrowIfLockNotAcquired();

      if (!Exists())
      {
        return;
      }

      Stopping?.Invoke(this, EventArgs.Empty);

      await _client.StopAsync(_container.ID, ct)
        .ConfigureAwait(false);

      _container = await _client.Container.ByIdAsync(_container.ID, ct)
        .ConfigureAwait(false);

      StoppedTime = DateTime.UtcNow;
      Stopped?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override bool Exists()
    {
      return _container != null && ContainerHasBeenCreatedStates.HasFlag(State);
    }
  }
}
