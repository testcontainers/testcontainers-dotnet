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
  using DotNet.Testcontainers.Internals;
  using JetBrains.Annotations;

  public class TestcontainersContainer : IDockerContainer
  {
    private static readonly TestcontainersState[] ContainerHasBeenCreatedStates = { TestcontainersState.Created, TestcontainersState.Running, TestcontainersState.Exited };

    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly ITestcontainersClient client;

    private readonly ITestcontainersConfiguration configuration;

    [NotNull]
    private ContainerListResponse container = new ContainerListResponse();

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

    protected TestcontainersContainer(ITestcontainersConfiguration configuration)
    {
      this.client = new TestcontainersClient(configuration.Endpoint);
      this.configuration = configuration;
    }

    ~TestcontainersContainer()
    {
      this.Dispose(false);
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

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public async Task<long> GetExitCode(CancellationToken ct = default)
    {
      await new SynchronizationContextRemover();
      await this.semaphoreSlim.WaitAsync(ct);

      try
      {
        return await this.client.GetContainerExitCode(this.Id, ct);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
      await new SynchronizationContextRemover();
      await this.semaphoreSlim.WaitAsync(ct);

      try
      {
        this.container = await this.Create(ct);
        this.container = await this.Start(this.Id, ct);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
      await new SynchronizationContextRemover();
      await this.semaphoreSlim.WaitAsync(ct);

      try
      {
        this.container = await this.Stop(this.Id, ct);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    public async Task CleanUpAsync(CancellationToken ct = default)
    {
      await new SynchronizationContextRemover();
      await this.semaphoreSlim.WaitAsync(ct);

      try
      {
        this.container = await this.CleanUp(this.Id, ct);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    public async Task<long> ExecAsync(IList<string> command, CancellationToken ct = default)
    {
      await new SynchronizationContextRemover();
      await this.semaphoreSlim.WaitAsync(ct);

      try
      {
        return await this.client.ExecAsync(this.Id, command, ct);
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    private async Task<ContainerListResponse> Create(CancellationToken ct = default)
    {
      if (ContainerHasBeenCreatedStates.Contains(this.State))
      {
        return this.container;
      }

      var id = await this.client.RunAsync(this.configuration, ct);
      return await this.client.GetContainer(id, ct);
    }

    private async Task<ContainerListResponse> Start(string id, CancellationToken ct = default)
    {
      using (var cts = new CancellationTokenSource())
      {
        var attachOutputConsumerTask = this.client.AttachAsync(id, this.configuration.OutputConsumer, cts.Token);

        var startTask = this.client.StartAsync(id, cts.Token);

        var waitTask = WaitStrategy.WaitUntil(() => this.configuration.WaitStrategy.Until(this.configuration.Endpoint, id), ct: cts.Token);

        var tasks = Task.WhenAll(attachOutputConsumerTask, startTask, waitTask);

        try
        {
          await tasks;
        }
        catch (Exception)
        {
          if (tasks.Exception != null)
          {
            throw tasks.Exception;
          }
        }
        finally
        {
          cts.Cancel();
        }
      }

      return await this.client.GetContainer(id, ct);
    }

    private async Task<ContainerListResponse> Stop(string id, CancellationToken ct = default)
    {
      await this.client.StopAsync(id, ct);
      return await this.client.GetContainer(id, ct);
    }

    private async Task<ContainerListResponse> CleanUp(string id, CancellationToken ct = default)
    {
      await this.client.RemoveAsync(id, ct);
      return new ContainerListResponse();
    }

    protected void Dispose(bool disposing)
    {
      if (!ContainerHasBeenCreatedStates.Contains(this.State))
      {
        return;
      }

      var cleanOrStopTask = this.configuration.CleanUp ? this.CleanUpAsync() : this.StopAsync();
      cleanOrStopTask.GetAwaiter().GetResult();
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
