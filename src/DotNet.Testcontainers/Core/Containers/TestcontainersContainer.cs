namespace DotNet.Testcontainers.Core.Containers
{
  using System;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Models;
  using LanguageExt;
  using static LanguageExt.Prelude;

  public class TestcontainersContainer : IDockerContainer
  {
    private Option<string> id = None;

    private Option<ContainerListResponse> container = None;

    internal TestcontainersContainer(TestcontainersConfiguration configuration, bool cleanUp = true)
    {
      this.Configuration = configuration;
      this.CleanUp = cleanUp;
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

    public void Start()
    {
      this.id = this.id.IfNone(TestcontainersClient.Instance.Run(this.Configuration));

      TestcontainersClient.Instance.Start(this.Id);

      this.id.IfSome(id =>
      {
        this.container = MetaDataClientContainers.Instance.ById(id);
      });
    }

    public void Stop()
    {
      TestcontainersClient.Instance.Stop(this.Id);

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
      this.id.IfSomeAsync(id =>
      {
        TestcontainersClient.Instance.Stop(id);

        if (this.CleanUp)
        {
          this.id = None;
          TestcontainersClient.Instance.Remove(id);
        }
      });
    }
  }
}
