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
    private Option<string> id = None;

    private Option<ContainerListResponse> container = None;

    internal TestcontainersContainer(TestcontainersConfiguration configuration)
    {
      this.Configuration = configuration;
      this.CleanUp = configuration.CleanUp;
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

    private bool CleanUp { get; }

    public async Task StartAsync()
    {
      this.id = await this.id.IfNoneAsync(() =>
      {
        return TestcontainersClient.Instance.RunAsync(this.Configuration);
      });

      await TestcontainersClient.Instance.StartAsync(this.Id);

      await WaitStrategy.WaitUntil(this.ContainerIsUpAndRunning);
    }

    public async Task StopAsync()
    {
      await TestcontainersClient.Instance.StopAsync(this.Id);

      this.id.IfSome(id =>
      {
        this.container = None;
      });
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      this.id.IfSome(async id =>
      {
        if (this.CleanUp)
        {
          this.id = None;
          this.container = None;
          await TestcontainersClient.Instance.RemoveAsync(id);
        }
        else
        {
          await TestcontainersClient.Instance.StopAsync(id);
        }
      });
    }

    private bool ContainerIsUpAndRunning()
    {
      this.container = MetaDataClientContainers.Instance.ByIdAsync(this.Id).GetAwaiter().GetResult();
      return this.container.Match(
        Some: value => !"Created".Equals(value.Status),
        None: () => throw new InvalidOperationException("Testcontainer not running."));
    }
  }
}
