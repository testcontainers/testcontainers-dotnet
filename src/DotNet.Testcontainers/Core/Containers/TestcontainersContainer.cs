namespace DotNet.Testcontainers.Core.Containers
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Core.Wait;

  public class TestcontainersContainer : IDockerContainer
  {
    private bool disposed;

    private string id;

    private ContainerListResponse container;

    protected TestcontainersContainer(TestcontainersConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    ~TestcontainersContainer()
    {
      this.Dispose(false);
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
        if (this.container == null)
        {
          throw new InvalidOperationException("Testcontainer is not running.");
        }

        return this.container.Names.FirstOrDefault() ?? string.Empty;
      }
    }

    public string IPAddress
    {
      get
      {
        if (this.container == null)
        {
          throw new InvalidOperationException("Testcontainer is not running.");
        }

        var ipAddress = this.container.NetworkSettings.Networks.FirstOrDefault();
        return ipAddress.Value == null ? string.Empty : ipAddress.Value.IPAddress;
      }
    }

    public string MacAddress
    {
      get
      {
        if (this.container == null)
        {
          throw new InvalidOperationException("Testcontainer is not running.");
        }

        var macAddress = this.container.NetworkSettings.Networks.FirstOrDefault();
        return macAddress.Value == null ? string.Empty : macAddress.Value.IPAddress;
      }
    }

    private TestcontainersConfiguration Configuration { get; }

    public async Task StartAsync()
    {
      await this.Create();

      await this.Start();

      this.container = await MetaDataClientContainers.Instance.ByIdAsync(this.Id);
    }

    public async Task StopAsync()
    {
      await this.Stop();
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

      if (this.Configuration.CleanUp)
      {
        Task.Run(this.CleanUp);
      }
      else
      {
        Task.Run(this.Stop);
      }

      this.disposed = true;
    }

    private async Task Create()
    {
      if (string.IsNullOrEmpty(this.Id))
      {
        this.id = await TestcontainersClient.Instance.RunAsync(this.Configuration);
      }
    }

    private Task Start()
    {
      if (string.IsNullOrEmpty(this.Id))
      {
        return Task.CompletedTask;
      }

      using (var cts = new CancellationTokenSource())
      {
        var attachConsumerTask = TestcontainersClient.Instance.AttachAsync(this.Id, this.Configuration.OutputConsumer, cts.Token);

        var startTask = TestcontainersClient.Instance.StartAsync(this.Id, cts.Token);

        var waitTask = WaitStrategy.WaitUntil(() => this.Configuration.WaitStrategy.Until(this.Id), cancellationToken: cts.Token);

        var handleDockerExceptionTask = startTask.ContinueWith(task =>
        {
          task.Exception?.Handle(exception =>
          {
            cts.Cancel();
            return false;
          });
        });

        return Task.WhenAll(attachConsumerTask, startTask, waitTask, handleDockerExceptionTask);
      }
    }

    private Task Stop()
    {
      if (string.IsNullOrEmpty(this.Id))
      {
        return Task.CompletedTask;
      }

      return TestcontainersClient.Instance.StopAsync(this.Id).ContinueWith(task =>
      {
        this.container = null;
      });
    }

    private Task CleanUp()
    {
      if (string.IsNullOrEmpty(this.Id))
      {
        return Task.CompletedTask;
      }

      return TestcontainersClient.Instance.RemoveAsync(this.Id).ContinueWith(task =>
      {
        this.container = null;
        this.id = null;
      });
    }
  }
}
