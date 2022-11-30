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

  /// <inheritdoc cref="ITestcontainersContainer" />
  public class TestcontainersContainer : ITestcontainersContainer
  {
    private const TestcontainersStates ContainerHasBeenCreatedStates = TestcontainersStates.Created | TestcontainersStates.Running | TestcontainersStates.Exited;

    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly ITestcontainersClient client;

    private readonly ITestcontainersConfiguration configuration;

    private int disposed;

    [NotNull]
    private ContainerInspectResponse container = new ContainerInspectResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersContainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    protected TestcontainersContainer(ITestcontainersConfiguration configuration, ILogger logger)
    {
      this.client = new TestcontainersClient(configuration.SessionId, configuration.DockerEndpointAuthConfig, logger);
      this.configuration = configuration;
      this.Logger = logger;
    }

    /// <inheritdoc />
    public string Id
    {
      get
      {
        this.ThrowIfContainerHasNotBeenCreated();
        return this.container.ID;
      }
    }

    /// <inheritdoc />
    public string Name
    {
      get
      {
        this.ThrowIfContainerHasNotBeenCreated();
        return this.container.Name;
      }
    }

    /// <inheritdoc />
    public string IpAddress
    {
      get
      {
        this.ThrowIfContainerHasNotBeenCreated();
        return this.container.NetworkSettings.Networks.First().Value.IPAddress;
      }
    }

    /// <inheritdoc />
    public string MacAddress
    {
      get
      {
        this.ThrowIfContainerHasNotBeenCreated();
        return this.container.NetworkSettings.Networks.First().Value.MacAddress;
      }
    }

    /// <inheritdoc />
    public string Hostname
    {
      get
      {
        var dockerHostUri = this.configuration.DockerEndpointAuthConfig.Endpoint;

        switch (dockerHostUri.Scheme)
        {
          case "http":
          case "https":
          case "tcp":
            return dockerHostUri.Host;
          case "npipe":
          case "unix":
            return this.GetContainerGateway();
          default:
            throw new InvalidOperationException($"Docker endpoint {dockerHostUri} is not supported.");
        }
      }
    }

    /// <inheritdoc />
    public IDockerImage Image
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
        this.ThrowIfContainerHasNotBeenCreated();
        return this.container.State.Health.FailingStreak;
      }
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    [NotNull]
    internal ILogger Logger { get; }

    /// <inheritdoc />
    public ushort GetMappedPublicPort(int privatePort)
    {
      return this.GetMappedPublicPort(Convert.ToString(privatePort, CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public ushort GetMappedPublicPort(string privatePort)
    {
      this.ThrowIfContainerHasNotBeenCreated();

      if (this.container.NetworkSettings.Ports.TryGetValue($"{privatePort}/tcp", out var portMap) && ushort.TryParse(portMap.First().HostPort, out var publicPort))
      {
        return publicPort;
      }
      else
      {
        throw new InvalidOperationException($"Exposed port {privatePort} is not mapped.");
      }
    }

    /// <inheritdoc />
    public Task<long> GetExitCode(CancellationToken ct = default)
    {
      return this.client.GetContainerExitCode(this.Id, ct);
    }

    /// <inheritdoc />
    public Task<(string Stdout, string Stderr)> GetLogs(DateTime since = default, DateTime until = default, CancellationToken ct = default)
    {
      return this.client.GetContainerLogs(this.Id, since, until, ct);
    }

    /// <inheritdoc />
    public virtual async Task StartAsync(CancellationToken ct = default)
    {
      await this.semaphoreSlim.WaitAsync(ct)
        .ConfigureAwait(false);

      try
      {
        this.container = await this.Create(ct)
          .ConfigureAwait(false);

        this.container = await this.Start(this.Id, ct)
          .ConfigureAwait(false);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken ct = default)
    {
      await this.semaphoreSlim.WaitAsync(ct)
        .ConfigureAwait(false);

      try
      {
        this.container = await this.Stop(this.Id, ct)
          .ConfigureAwait(false);
      }
      catch (DockerContainerNotFoundException)
      {
        this.container = new ContainerInspectResponse();
      }
      finally
      {
        this.semaphoreSlim.Release();
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

    /// <summary>
    /// Removes the Testcontainers.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous clean up operation of a Testcontainers.</returns>
    public async Task CleanUpAsync(CancellationToken ct = default)
    {
      await this.semaphoreSlim.WaitAsync(ct)
        .ConfigureAwait(false);

      try
      {
        this.container = await this.CleanUp(this.Id, ct)
          .ConfigureAwait(false);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
      await this.DisposeAsyncCore()
        .ConfigureAwait(false);

      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases any resources associated with the instance of <see cref="TestcontainersContainer" />.
    /// </summary>
    /// <returns>Value task that completes when any resources associated with the instance have been released.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
      if (1.Equals(Interlocked.CompareExchange(ref this.disposed, 1, 0)))
      {
        return;
      }

      if (!ContainerHasBeenCreatedStates.HasFlag(this.State))
      {
        return;
      }

      // If someone calls `DisposeAsync`, we can immediately remove the container. We do not need to wait for the Resource Reaper.
      if (Guid.Empty.Equals(this.configuration.SessionId))
      {
        await this.StopAsync()
          .ConfigureAwait(false);
      }
      else
      {
        await this.CleanUpAsync()
          .ConfigureAwait(false);
      }

      this.semaphoreSlim.Dispose();
    }

    private async Task<ContainerInspectResponse> Create(CancellationToken ct = default)
    {
      if (ContainerHasBeenCreatedStates.HasFlag(this.State))
      {
        return this.container;
      }

      var id = await this.client.RunAsync(this.configuration, ct)
        .ConfigureAwait(false);

      return await this.client.InspectContainer(id, ct)
        .ConfigureAwait(false);
    }

    private async Task<ContainerInspectResponse> Start(string id, CancellationToken ct = default)
    {
      await this.client.AttachAsync(id, this.configuration.OutputConsumer, ct)
        .ConfigureAwait(false);

      await this.client.StartAsync(id, ct)
        .ConfigureAwait(false);

      this.container = await this.client.InspectContainer(id, ct)
        .ConfigureAwait(false);

      await this.configuration.StartupCallback(this, ct)
        .ConfigureAwait(false);

      // Do not use a too small frequency. Especially with a lot of containers,
      // we send many operations to the Docker endpoint. The endpoint may cancel operations.
      var frequency = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;

      const int timeout = -1;

      foreach (var waitStrategy in this.configuration.WaitStrategies)
      {
        await WaitStrategy.WaitUntil(
            async () =>
            {
              this.container = await this.client.InspectContainer(id, ct)
                .ConfigureAwait(false);

              return await waitStrategy.Until(this, this.Logger)
                .ConfigureAwait(false);
            },
            frequency,
            timeout,
            ct)
          .ConfigureAwait(false);
      }

      return this.container;
    }

    private async Task<ContainerInspectResponse> Stop(string id, CancellationToken ct = default)
    {
      await this.client.StopAsync(id, ct)
        .ConfigureAwait(false);

      return await this.client.InspectContainer(id, ct)
        .ConfigureAwait(false);
    }

    private async Task<ContainerInspectResponse> CleanUp(string id, CancellationToken ct = default)
    {
      await this.client.RemoveAsync(id, ct)
        .ConfigureAwait(false);

      return new ContainerInspectResponse();
    }

    private void ThrowIfContainerHasNotBeenCreated()
    {
      if (ContainerHasBeenCreatedStates.HasFlag(this.State))
      {
        return;
      }

      throw new InvalidOperationException("Container has not been created.");
    }

    private string GetContainerGateway()
    {
      const string localhost = "127.0.0.1";

      if (!ContainerHasBeenCreatedStates.HasFlag(this.State))
      {
        return localhost;
      }

      if (!this.client.IsRunningInsideDocker)
      {
        return localhost;
      }

      var endpointSettings = this.container.NetworkSettings.Networks.Values.FirstOrDefault();
      return endpointSettings == null ? localhost : endpointSettings.Gateway;
    }
  }
}
