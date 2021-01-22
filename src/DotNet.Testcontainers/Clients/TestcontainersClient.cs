namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Images.Configurations;
  using DotNet.Testcontainers.Services;
  using ICSharpCode.SharpZipLib.Tar;

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
      new DockerSystemOperations(endpoint))
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

      AppDomain.CurrentDomain.ProcessExit += this.PurgeOrphanedContainers;
      Console.CancelKeyPress += this.PurgeOrphanedContainers;
    }

    ~TestcontainersClient()
    {
      this.Dispose();
    }

    public bool IsRunningInsideDocker
    {
      get
      {
        return File.Exists(Path.Combine(this.osRootDirectory, ".dockerenv"));
      }
    }

    public void Dispose()
    {
      AppDomain.CurrentDomain.ProcessExit -= this.PurgeOrphanedContainers;
      Console.CancelKeyPress -= this.PurgeOrphanedContainers;
      GC.SuppressFinalize(this);
    }

    public Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default)
    {
      return this.system.GetIsWindowsEngineEnabled(ct);
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
      if (await this.containers.ExistsWithIdAsync(id, ct)
        .ConfigureAwait(false))
      {
        await this.containers.StartAsync(id, ct)
          .ConfigureAwait(false);
      }
    }

    public async Task StopAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct)
        .ConfigureAwait(false))
      {
        await this.containers.StopAsync(id, ct)
          .ConfigureAwait(false);
      }
    }

    public async Task RemoveAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct)
        .ConfigureAwait(false))
      {
        await this.containers.RemoveAsync(id, ct)
          .ConfigureAwait(false);
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

    public async Task CopyFileAsync(string id, string filePath, byte[] fileContent, int accessMode, int userId, int groupId, CancellationToken ct = default)
    {
      await using (var memStream = new MemoryStream())
      {
        await using (var tarOutputStream = new TarOutputStream(memStream, Encoding.Default))
        {
          tarOutputStream.IsStreamOwner = false;
          tarOutputStream.PutNextEntry(
            new TarEntry(
              new TarHeader
              {
                Name = filePath,
                UserId = userId,
                GroupId = groupId,
                Mode = accessMode,
                Size = fileContent.Length
              }));

          await tarOutputStream.WriteAsync(fileContent, ct)
            .ConfigureAwait(false);

          tarOutputStream.CloseEntry();
        }

        memStream.Seek(0, SeekOrigin.Begin);

        await this.containers.ExtractArchiveToContainerAsync(id, "/", memStream, ct)
          .ConfigureAwait(false);
      }
    }

    public async Task<string> RunAsync(ITestcontainersConfiguration configuration, CancellationToken ct = default)
    {
      if (!await this.images.ExistsWithNameAsync(configuration.Image.FullName, ct)
        .ConfigureAwait(false))
      {
        await this.images.CreateAsync(configuration.Image, configuration.AuthConfig, ct)
          .ConfigureAwait(false);
      }

      var id = await this.containers.RunAsync(configuration, ct)
        .ConfigureAwait(false);

      this.registryService.Register(id, configuration.CleanUp);

      return id;
    }

    public Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default)
    {
      return this.images.BuildAsync(configuration, ct);
    }

    private void PurgeOrphanedContainers(object sender, EventArgs args)
    {
      var arguments = new PurgeOrphanedContainersArgs(this.endpoint, this.registryService.GetRegisteredContainers());
      new Process { StartInfo = { FileName = "docker", Arguments = arguments.ToString() } }.Start();
    }
  }
}
