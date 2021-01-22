namespace DotNet.Testcontainers.Containers.Modules
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using JetBrains.Annotations;

  public class TestcontainersContainer : IDockerContainer
  {
    private static readonly TestcontainersState[] ContainerHasBeenCreatedStates = { TestcontainersState.Created, TestcontainersState.Running, TestcontainersState.Exited };

    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly ITestcontainersClient client;

    private readonly ITestcontainersConfiguration configuration;

    [NotNull]
    private ContainerListResponse container = new ContainerListResponse();

    protected TestcontainersContainer(ITestcontainersConfiguration configuration)
    {
      this.client = new TestcontainersClient(configuration.Endpoint);
      this.configuration = configuration;
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
        return this.container.Names.First();
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
        switch (this.configuration.Endpoint.Scheme)
        {
          case "tcp":
          case "http":
          case "https":
            return this.configuration.Endpoint.Host;
          case "unix":
          case "npipe":
            if (this.client.IsRunningInsideDocker)
            {
              this.ThrowIfContainerHasNotBeenCreated();
              return this.container.NetworkSettings.Networks.First().Value.Gateway;
            }
            else
            {
              return "localhost";
            }
          default:
            return this.IpAddress;
        }
      }
    }

    private TestcontainersState State
    {
      get
      {
        try
        {
          return (TestcontainersState)Enum.Parse(typeof(TestcontainersState), this.container.State, true);
        }
        catch (Exception)
        {
          return TestcontainersState.Undefined;
        }
      }
    }

    public ushort GetMappedPublicPort(int privatePort)
    {
      return this.GetMappedPublicPort($"{privatePort}");
    }

    public ushort GetMappedPublicPort(string privatePort)
    {
      this.ThrowIfContainerHasNotBeenCreated();
      var mappedPort = this.container.Ports.FirstOrDefault(port => $"{port.PrivatePort}".Equals(privatePort));
      return mappedPort?.PublicPort ?? ushort.MinValue;
    }

    public async Task<long> GetExitCode(CancellationToken ct = default)
    {
      await this.semaphoreSlim.WaitAsync(ct)
        .ConfigureAwait(false);

      try
      {
        return await this.client.GetContainerExitCode(this.Id, ct)
          .ConfigureAwait(false);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    public async Task StartAsync(CancellationToken ct = default)
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

    public async Task StopAsync(CancellationToken ct = default)
    {
      await this.semaphoreSlim.WaitAsync(ct)
        .ConfigureAwait(false);

      try
      {
        this.container = await this.Stop(this.Id, ct)
          .ConfigureAwait(false);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

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

    public Task CopyFileAsync(string filePath, byte[] fileContent, int accessMode = 384, int userId = 0, int groupId = 0, CancellationToken ct = default)
    {
      return this.client.CopyFileAsync(this.Id, filePath, fileContent, accessMode, userId, groupId, ct);
    }

    public Task<long> ExecAsync(IList<string> command, CancellationToken ct = default)
    {
      return this.client.ExecAsync(this.Id, command, ct);
    }

    public virtual async ValueTask DisposeAsync()
    {
      if (!ContainerHasBeenCreatedStates.Contains(this.State))
      {
        return;
      }

      var cleanOrStopTask = this.configuration.CleanUp ? this.CleanUpAsync() : this.StopAsync();
      await cleanOrStopTask.ConfigureAwait(false);

      this.semaphoreSlim.Dispose();
      this.client.Dispose();
    }

    private async Task<ContainerListResponse> Create(CancellationToken ct = default)
    {
      if (ContainerHasBeenCreatedStates.Contains(this.State))
      {
        return this.container;
      }

      var id = await this.client.RunAsync(this.configuration, ct)
        .ConfigureAwait(false);
      return await this.client.GetContainer(id, ct)
        .ConfigureAwait(false);
    }

    private async Task<ContainerListResponse> Start(string id, CancellationToken ct = default)
    {
      await this.client.AttachAsync(id, this.configuration.OutputConsumer, ct)
        .ConfigureAwait(false);
      await this.client.StartAsync(id, ct)
        .ConfigureAwait(false);

      this.container = await this.client.GetContainer(id, ct)
        .ConfigureAwait(false);

      await this.configuration.StartupCallback(this, ct)
        .ConfigureAwait(false);

      foreach (var waitStrategy in this.configuration.WaitStrategies)
      {
        await WaitStrategy.WaitUntil(() => waitStrategy.Until(this.configuration.Endpoint, id), 100, ct: ct)
          .ConfigureAwait(false);
      }

      return await this.client.GetContainer(id, ct)
        .ConfigureAwait(false);
    }

    private async Task<ContainerListResponse> Stop(string id, CancellationToken ct = default)
    {
      await this.client.StopAsync(id, ct)
        .ConfigureAwait(false);
      return await this.client.GetContainer(id, ct)
        .ConfigureAwait(false);
    }

    private async Task<ContainerListResponse> CleanUp(string id, CancellationToken ct = default)
    {
      await this.client.RemoveAsync(id, ct)
        .ConfigureAwait(false);
      return new ContainerListResponse();
    }

    private void ThrowIfContainerHasNotBeenCreated()
    {
      if (!ContainerHasBeenCreatedStates.Contains(this.State))
      {
        throw new InvalidOperationException("Testcontainer has not been created.");
      }
    }
  }
}
