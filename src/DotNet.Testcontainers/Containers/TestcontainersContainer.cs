namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Images;
  using LanguageExt;
  using static LanguageExt.Prelude;

  public class TestcontainersContainer : IDockerContainer
  {
    private readonly Option<string> name;

    private bool disposed = false;

    public TestcontainersContainer(
      string name,
      IDockerImage image,
      IReadOnlyDictionary<string, string> exposedPorts,
      IReadOnlyDictionary<string, string> portBindings,
      IReadOnlyDictionary<string, string> volumes,
      bool cleanUp = true)
    {
      this.name = Optional(name);
      this.Image = image;
      this.ExposedPorts = exposedPorts;
      this.PortBindings = portBindings;
      this.Volumes = volumes;
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
        return this.name.Match(
          Some: name => name,
          None: () => TestcontainersClient.Instance.FindContainerNameById(this.Id));
      }
    }

    private bool CleanUp { get; }

    private IDockerImage Image { get; }

    private IReadOnlyDictionary<string, string> ExposedPorts { get; }

    private IReadOnlyDictionary<string, string> PortBindings { get; }

    private IReadOnlyDictionary<string, string> Volumes { get; }

    private HostConfig HostConfig
    {
      get
      {
        var portBindings = this.PortBindings.ToDictionary(binding => $"{binding.Value}/tcp", binding => (IList<PortBinding>)List(new PortBinding { HostPort = binding.Key }).ToList());

        return new HostConfig
        {
          PortBindings = portBindings,
        };
      }
    }

    public void Start()
    {
      if (!TestcontainersClient.Instance.ExistImageByName(this.Image.Image))
      {
        TestcontainersClient.Instance.Pull(this.Image.Image);
      }

      if (!TestcontainersClient.Instance.ExistContainerById(this.Id))
      {
        this.Id = TestcontainersClient.Instance.Run(this.Name, this.Image.Image, this.HostConfig);
      }

      TestcontainersClient.Instance.Start(this.Id);
    }

    public void Stop()
    {
      if (TestcontainersClient.Instance.ExistContainerById(this.Id))
      {
        TestcontainersClient.Instance.Stop(this.Id);
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        this.Stop();

        if (this.CleanUp)
        {
          TestcontainersClient.Instance.Remove(this.Id);
        }

        this.disposed = true;
      }
    }
  }
}
