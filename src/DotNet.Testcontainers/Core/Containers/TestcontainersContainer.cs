namespace DotNet.Testcontainers.Core.Containers
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Models;
  using LanguageExt;
  using static LanguageExt.Prelude;

  public class TestcontainersContainer : IDockerContainer
  {
    private static readonly DefaultWaitStrategy Wait = new DefaultWaitStrategy();

    private Option<string> id = None;

    private Option<ContainerListResponse> container = None;

    internal TestcontainersContainer(TestcontainersConfiguration configuration)
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
      if (this.Configuration.CleanUp)
      {
        Task.Run(this.CleanUp);
      }
      else
      {
        Task.Run(this.Stop);
      }
    }

    private async Task Create()
    {
      this.id = await this.id.IfNoneAsync(() =>
      {
        return TestcontainersClient.Instance.RunAsync(this.Configuration);
      });
    }

    private async Task Start()
    {
      await this.id.IfSomeAsync(async id =>
      {
        var startTask = TestcontainersClient.Instance.StartAsync(id);

        var waitTask = this.Configuration.WaitStrategy?.WaitUntil() ?? Wait.ForContainer(id).WaitUntil();

        await Task.WhenAll(startTask, waitTask);
      });
    }

    private async Task Stop()
    {
      await this.id.IfSomeAsync(async id =>
      {
        this.container = None;
        await TestcontainersClient.Instance.StopAsync(id);
      });
    }

    private async Task CleanUp()
    {
      await this.id.IfSomeAsync(async id =>
      {
        this.id = None;
        this.container = None;
        await TestcontainersClient.Instance.RemoveAsync(id);
      });
    }
  }
}
