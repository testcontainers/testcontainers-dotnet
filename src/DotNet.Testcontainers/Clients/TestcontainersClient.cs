namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Images.Configurations;
  using DotNet.Testcontainers.Internals;
  using DotNet.Testcontainers.Services;

  internal sealed class TestcontainersClient : ITestcontainersClient
  {
    private readonly string osRootDirectory = Path.GetPathRoot(typeof(ITestcontainersClient).Assembly.Location);

    private readonly Uri endpoint;

    private readonly TestcontainersRegistryService registryService;

    private readonly IDockerContainerOperations containers;

    private readonly IDockerImageOperations images;

    private readonly IDockerSystemOperations system;

    public TestcontainersClient() : this(
      DockerApiEndpoint.Local)
    {
    }

    public TestcontainersClient(Uri endpoint) : this(
      new TestcontainersRegistryService(),
      new DockerContainerOperations(endpoint),
      new DockerImageOperations(endpoint),
      new DockerSystemOperations(endpoint)
      )
    {
      this.endpoint = endpoint;
    }

    private TestcontainersClient(
      TestcontainersRegistryService registryService,
      IDockerContainerOperations containerOperations,
      IDockerImageOperations imageOperations,
      IDockerSystemOperations systemOperations)
    {
      this.registryService = registryService;
      this.containers = containerOperations;
      this.images = imageOperations;
      this.system = systemOperations;

      AppDomain.CurrentDomain.ProcessExit += (sender, args) => this.PurgeOrphanedContainers();
      Console.CancelKeyPress += (sender, args) => this.PurgeOrphanedContainers();
    }

    public bool IsRunningInsideDocker
    {
      get
      {
        return File.Exists(Path.Combine(this.osRootDirectory, ".dockerenv"));
      }
    }

    public async Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default)
    {
      await new SynchronizationContextRemover();
      return await this.system.GetIsWindowsEngineEnabled(ct);
    }

    public Task<ContainerListResponse> GetContainer(string id, CancellationToken ct = default)
    {
      return this.containers.ByIdAsync(id, ct);
    }

    public Task<long> GetContainerExitCode(string id, CancellationToken ct = default)
    {
      return this.containers.GetExitCode(id, ct);
    }

    public async Task StartAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct))
      {
        await this.containers.StartAsync(id, ct);
      }
    }

    public async Task StopAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct))
      {
        await this.containers.StopAsync(id, ct);
      }
    }

    public async Task RemoveAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct))
      {
        await this.containers.RemoveAsync(id, ct);
      }

      this.registryService.Unregister(id);
    }

    public Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default)
    {
      return this.containers.AttachAsync(id, outputConsumer, ct);
    }

    public Task<long> ExecAsync(string id, IList<string> command, CancellationToken ct = default)
    {
      return this.containers.ExecAsync(id, command, ct);
    }

    public async Task<string> RunAsync(ITestcontainersConfiguration configuration, CancellationToken ct = default)
    {
      if (!await this.images.ExistsWithNameAsync(configuration.Image.FullName, ct))
      {
        await this.images.CreateAsync(configuration.Image, ct);
      }

      var id = await this.containers.RunAsync(configuration, ct);
      this.registryService.Register(id, configuration.CleanUp);

      return id;
    }

    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default)
    {
      return await this.images.BuildAsync(configuration, ct);
    }

    private void PurgeOrphanedContainers()
    {
      var args = new PurgeOrphanedContainersArgs(this.endpoint, this.registryService.GetRegisteredContainers());
      new Process { StartInfo = { FileName = "docker", Arguments = args.ToString() } }.Start();
    }
  }
}
