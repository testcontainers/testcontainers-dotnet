namespace DotNet.Testcontainers.Core.Containers
{
  using System;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Models;
  using LanguageExt;
  using static LanguageExt.Prelude;

  public class TestcontainersContainer : IDockerContainer
  {
    private Option<string> id;

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

      set
      {
        this.id = Some(value);
      }
    }

    public string Name
    {
      get
      {
        return this.id.Match(
          Some: TestcontainersClient.Instance.FindContainerNameById,
          None: () => $"/{this.Configuration.Container.Name}");
      }
    }

    private TestcontainersConfiguration Configuration { get; }

    private bool CleanUp { get; }

    public void Start()
    {
      this.id = this.id.IfNone(TestcontainersClient.Instance.Run(this.Configuration));

      TestcontainersClient.Instance.Start(this.Id);
    }

    public void Stop()
    {
      TestcontainersClient.Instance.Stop(this.Id);
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
        if (!this.CleanUp)
        {
          TestcontainersClient.Instance.Stop(id);
        }
        else
        {
          this.id = None;
          TestcontainersClient.Instance.Remove(id);
        }
      });
    }
  }
}
