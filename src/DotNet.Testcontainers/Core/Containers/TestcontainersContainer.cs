namespace DotNet.Testcontainers.Core.Containers
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Models;
  using LanguageExt;
  using static LanguageExt.Prelude;

  public class TestcontainersContainer : IDockerContainer
  {
    private bool disposed;

    private Option<string> id = None;

    private Option<ContainerListResponse> container = None;

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
        return this.id.IfNone(() => string.Empty);
      }
    }

    public string Name
    {
      get
      {
        return this.container.Match(
          Some: value => value.Names.FirstOrDefault() ?? string.Empty,
          None: () => throw new InvalidOperationException("Testcontainer not running."));
      }
    }

    public string IPAddress
    {
      get
      {
        return this.container.Match(
          Some: value =>
          {
            var ipAddress = value.NetworkSettings.Networks.FirstOrDefault();
            return notnull(ipAddress) ? ipAddress.Value.IPAddress : string.Empty;
          },
          None: () => throw new InvalidOperationException("Testcontainer not running."));
      }
    }

    public string MacAddress
    {
      get
      {
        return this.container.Match(
          Some: value =>
          {
            var macAddress = value.NetworkSettings.Networks.FirstOrDefault();
            return notnull(macAddress) ? macAddress.Value.MacAddress : string.Empty;
          },
          None: () => throw new InvalidOperationException("Testcontainer not running."));
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
      this.id = await this.id.IfNoneAsync(() =>
      {
        return TestcontainersClient.Instance.RunAsync(this.Configuration);
      });
    }

    private Task Start()
    {
      return this.id.IfSomeAsync(id =>
      {
        var cts = new CancellationTokenSource();

        var attachConsumerTask = TestcontainersClient.Instance.AttachAsync(id, this.Configuration.OutputConsumer, cts.Token);

        var startTask = TestcontainersClient.Instance.StartAsync(id, cts.Token);

        var waitTask = WaitStrategy.WaitUntil(() => { return this.Configuration.WaitStrategy.Until(id); }, cancellationToken: cts.Token);

        var handleDockerExceptionTask = startTask.ContinueWith((task) =>
        {
          if (task.Exception != null)
          {
            task.Exception.Handle(exception =>
            {
              cts.Cancel();
              return false;
            });
          }
        });

        return Task.WhenAll(attachConsumerTask, startTask, waitTask, handleDockerExceptionTask);
      });
    }

    private Task Stop()
    {
      return this.id.IfSomeAsync(id =>
      {
        this.container = None;
        return TestcontainersClient.Instance.StopAsync(id);
      });
    }

    private Task CleanUp()
    {
      return this.id.IfSomeAsync(id =>
      {
        this.id = None;
        this.container = None;
        return TestcontainersClient.Instance.RemoveAsync(id);
      });
    }
  }
}
