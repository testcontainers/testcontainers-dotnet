namespace DotNet.Testcontainers.Core.Container
{
  using System;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Builder;
  using static LanguageExt.Prelude;

  public class TestcontainersContainer : IDockerContainer
  {
    internal TestcontainersContainer(
      DockerContainerConfig containerConfig,
      DockerHostConfig hostConfig,
      bool cleanUp = true)
    {
      this.ContainerConfig = containerConfig;
      this.HostConfig = hostConfig;
      this.CleanUp = cleanUp;
    }

    ~TestcontainersContainer()
    {
      this.Dispose(false);
    }

    public string Id { get; private set; }

    public string Name
    {
      get
      {
        return Optional(this.ContainerConfig.Name).IfNone(TestcontainersClient.Instance.FindContainerNameById(this.Id));
      }
    }

    private DockerContainerConfig ContainerConfig { get; }

    private DockerHostConfig HostConfig { get; }

    private bool CleanUp { get; }

    public void Start()
    {
      this.Id = Optional(this.Id).IfNone(
        TestcontainersClient.Instance.Run(
          this.ContainerConfig,
          this.HostConfig));

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
      Optional(this.Id).IfSomeAsync(id =>
      {
        if (!this.CleanUp)
        {
          TestcontainersClient.Instance.Stop(id);
        }
        else
        {
          this.Id = null;
          TestcontainersClient.Instance.Remove(id);
        }
      });
    }
  }
}
