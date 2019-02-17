namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.InteropServices;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Diagnostics;
  using DotNet.Testcontainers.Images;

  using static DotNet.Testcontainers.Collections.Collections;

  // TODO: Fix synchrone implementation of Docker commands!
  public class GenericContainer : IDisposable
  {
    private static readonly Uri Endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");

    private static readonly DockerClient Client = new DockerClientConfiguration(Endpoint).CreateClient();

    private bool disposed = false;

    public GenericContainer(
      GenericImage image,
      IDictionary<string, string> exposedPorts,
      IDictionary<string, string> portBindings)
    {
      this.Image = image;
      this.ExposedPorts = exposedPorts;
      this.PortBindings = portBindings;
    }

    ~GenericContainer()
    {
      this.Dispose(false);
    }

    public string Id { get; set; }

    private GenericImage Image { get; }

    private IDictionary<string, string> ExposedPorts { get; }

    private IDictionary<string, string> PortBindings { get; }

    private HostConfig HostConfig
    {
      get
      {
        var portBindings = this.PortBindings.ToDictionary(binding => $"{binding.Key}/tcp", binding => ListOf(new PortBinding { HostPort = binding.Value }));

        return new HostConfig
        {
          PortBindings = portBindings,
        };
      }
    }

    public void Pull()
    {
      Client.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = this.Image.Image }, null, DebugProgress.Instance).Wait();
    }

    public void Run()
    {
      var response = Client.Containers.CreateContainerAsync(new CreateContainerParameters
      {
        Image = this.Image.Image,
        HostConfig = this.HostConfig,
      }).Result;

      this.Id = response.ID;
    }

    public void Start()
    {
      Client.Containers.StartContainerAsync(this.Id, new ContainerStartParameters { }).Wait();
    }

    public void Stop()
    {
      Client.Containers.StopContainerAsync(this.Id, new ContainerStopParameters { WaitBeforeKillSeconds = 30 }).Wait();
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

        this.disposed = true;
      }
    }
  }
}
