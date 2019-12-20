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

  /// <summary>
  /// This class represents a configured and created Testcontainer.
  /// </summary>
  public class TestcontainersContainer : IDockerContainer
  {
    private const string ContainerIsNotRunning = "Testcontainer is not running.";

    private readonly ITestcontainersClient client;

    private readonly ITestcontainersConfiguration configuration;

    private bool disposed;

    private string id;

    private ContainerListResponse container;

    internal TestcontainersContainer(ITestcontainersConfiguration configuration)
    {
      this.client = new TestcontainersClient(configuration.Endpoint);
      this.configuration = configuration;
    }

    ~TestcontainersContainer()
    {
      this.Dispose(false);
    }

    public bool HasId
    {
      get
      {
        return !string.IsNullOrEmpty(this.id);
      }
    }

    public string Id
    {
      get
      {
        return this.id ?? string.Empty;
      }
    }

    public string Name
    {
      get
      {
        this.ThrowIfContainerIsNotRunning();
        return this.container.Names.FirstOrDefault() ?? string.Empty;
      }
    }

    public string IpAddress
    {
      get
      {
        this.ThrowIfContainerIsNotRunning();
        var ipAddress = this.container.NetworkSettings.Networks.FirstOrDefault();
        return ipAddress.Value?.IPAddress ?? string.Empty;
      }
    }

    public string MacAddress
    {
      get
      {
        this.ThrowIfContainerIsNotRunning();
        var macAddress = this.container.NetworkSettings.Networks.FirstOrDefault();
        return macAddress.Value?.MacAddress ?? string.Empty;
      }
    }

    public ushort GetMappedPublicPort(int privatePort)
    {
      return this.GetMappedPublicPort($"{privatePort}");
    }

    public ushort GetMappedPublicPort(string privatePort)
    {
      this.ThrowIfContainerIsNotRunning();
      var mappedPort = this.container.Ports.FirstOrDefault(port => $"{port.PrivatePort}".Equals(privatePort));
      return mappedPort?.PublicPort ?? ushort.MinValue;
    }

    public Task<long> GetExitCode()
    {
      return this.client.GetContainerExitCode(this.Id);
    }

    public async Task StartAsync()
    {
      await this.Create();
      await this.Start();
      this.container = await this.client.GetContainer(this.Id);
    }

    public async Task StopAsync()
    {
      await this.Stop();
    }

    public Task<long> ExecAsync(IList<string> command, CancellationToken ct = default)
    {
      this.ThrowIfContainerIsNotRunning();
      return this.client.ExecAsync(this.Id, command, ct);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      var cleanOrStopTask = this.configuration.CleanUp ? this.CleanUp() : this.Stop();
      cleanOrStopTask.GetAwaiter().GetResult();

      this.disposed = true;
    }

    private void ThrowIfContainerIsNotRunning()
    {
      if (this.container == null)
      {
        throw new InvalidOperationException(ContainerIsNotRunning);
      }
    }

    private async Task Create()
    {
      if (!this.HasId)
      {
        this.id = await this.client.RunAsync(this.configuration);
      }
    }

    private async Task Start()
    {
      if (this.HasId)
      {
        using (var cts = new CancellationTokenSource())
        {
          var attachOutputConsumerTask = this.client.AttachAsync(this.Id, this.configuration.OutputConsumer, cts.Token);

          var startTask = this.client.StartAsync(this.Id, cts.Token);

          var waitTask = WaitStrategy.WaitUntil(() => this.configuration.WaitStrategy.Until(this.configuration.Endpoint, this.Id), ct: cts.Token);

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
      }
    }

    private async Task Stop()
    {
      if (this.HasId)
      {
        await this.client.StopAsync(this.Id);
        this.container = null;
      }
    }

    private async Task CleanUp()
    {
      if (this.HasId)
      {
        await this.client.RemoveAsync(this.Id);
        this.id = null;
        this.container = null;
      }
    }
  }
}
