namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Runtime.InteropServices;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Diagnostics;
  using DotNet.Testcontainers.Images;

  public class GenericContainer : IDisposable
  {
    private static readonly Uri Endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");

    private static readonly DockerClient Client = new DockerClientConfiguration(Endpoint).CreateClient();

    private bool disposed = false;

    public GenericContainer(GenericImage image)
    {
      this.Image = image;
    }

    ~GenericContainer()
    {
      this.Dispose(false);
    }

    public GenericImage Image { get; set; }

    public string Id { get; set; }

    public void Pull()
    {
      Client.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = this.Image.Image }, null, DebugProgress.Instance);
    }

    public void Run()
    {
      var response = Client.Containers.CreateContainerAsync(new CreateContainerParameters { Image = this.Image.Image }).Result;
      this.Id = response.ID;
    }

    public void Start()
    {
      Client.Containers.StartContainerAsync(this.Id, new ContainerStartParameters { });
    }

    public void Stop()
    {
      Client.Containers.StopContainerAsync(this.Id, new ContainerStopParameters { WaitBeforeKillSeconds = 30 });
    }

    public void Dispose()
    {
      this.Dispose(true);       GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
        }

        this.Stop();

        this.disposed = true;
      }
    }
  }
}
