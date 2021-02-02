namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Internals.Mappers;
  using DotNet.Testcontainers.Services;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerContainerOperations : DockerApiClient, IDockerContainerOperations
  {
    private static readonly ILogger<DockerContainerOperations> Logger = TestcontainersHostService.GetLogger<DockerContainerOperations>();

    public DockerContainerOperations(Uri endpoint) : base(endpoint)
    {
    }

    public async Task<IEnumerable<ContainerListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await this.Docker.Containers.ListContainersAsync(new ContainersListParameters { All = true }, ct)
        .ConfigureAwait(false)).ToArray();
    }

    public async Task<IEnumerable<ContainerListResponse>> GetOrphanedObjects(CancellationToken ct = default)
    {
      var filters = new FilterByProperty { { "label", $"{TestcontainersClient.TestcontainersCleanUpLabel}=true" }, { "status", "exited" } };
      return (await this.Docker.Containers.ListContainersAsync(new ContainersListParameters { All = true, Filters = filters }, ct)
        .ConfigureAwait(false)).ToArray();
    }

    public Task<ContainerListResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return this.ByPropertyAsync("id", id, ct);
    }

    public Task<ContainerListResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return this.ByPropertyAsync("name", name, ct);
    }

    public async Task<ContainerListResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var filters = new FilterByProperty(property, value);
      return (await this.Docker.Containers.ListContainersAsync(new ContainersListParameters { All = true, Filters = filters }, ct)
        .ConfigureAwait(false)).FirstOrDefault();
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      return await this.ByIdAsync(id, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default)
    {
      return await this.ByNameAsync(name, ct)
        .ConfigureAwait(false) != null;
    }

    public async Task<long> GetExitCode(string id, CancellationToken ct = default)
    {
      return (await this.Docker.Containers.WaitContainerAsync(id, ct)
        .ConfigureAwait(false)).StatusCode;
    }

    public Task StartAsync(string id, CancellationToken ct = default)
    {
      Logger.LogInformation("Starting container {id}", id);
      return this.Docker.Containers.StartContainerAsync(id, new ContainerStartParameters(), ct);
    }

    public Task StopAsync(string id, CancellationToken ct = default)
    {
      Logger.LogInformation("Stopping container {id}", id);
      return this.Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }, ct);
    }

    public Task RemoveAsync(string id, CancellationToken ct = default)
    {
      Logger.LogInformation("Removing container {id}", id);
      return this.Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true, RemoveVolumes = true }, ct);
    }

    public Task ExtractArchiveToContainerAsync(string id, string path, Stream tarStream, CancellationToken ct = default)
    {
      Logger.LogInformation("Copying tar stream to {path} at container {id}", path, id);
      return this.Docker.Containers.ExtractArchiveToContainerAsync(id, new ContainerPathStatParameters { Path = path, AllowOverwriteDirWithFile = false }, tarStream, ct);
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default)
    {
      Logger.LogInformation("Attaching {outputConsumer} at container {id}", outputConsumer.GetType(), id);

      var attachParameters = new ContainerAttachParameters
      {
        Stdout = true,
        Stderr = true,
        Stream = true,
      };

      var stream = await this.Docker.Containers.AttachContainerAsync(id, false, attachParameters, ct)
        .ConfigureAwait(false);

      _ = stream.CopyOutputToAsync(Stream.Null, outputConsumer.Stdout, outputConsumer.Stderr, ct)
        .ConfigureAwait(false);
    }

    public async Task<long> ExecAsync(string id, IList<string> command, CancellationToken ct = default)
    {
      Logger.LogInformation("Executing {command} at container {id}", command, id);

      var created = await this.Docker.Exec.ExecCreateContainerAsync(id, new ContainerExecCreateParameters { Cmd = command }, ct)
        .ConfigureAwait(false);

      await this.Docker.Exec.StartContainerExecAsync(created.ID, ct)
        .ConfigureAwait(false);

      for (ContainerExecInspectResponse response; (response = await this.Docker.Exec.InspectContainerExecAsync(created.ID, ct)
        .ConfigureAwait(false)) != null;)
      {
        if (!response.Running)
        {
          return response.ExitCode;
        }
      }

      return long.MinValue;
    }

    public async Task<string> RunAsync(ITestcontainersConfiguration configuration, CancellationToken ct = default)
    {
      var converter = new TestcontainersConfigurationConverter(configuration);

      var hostConfig = new HostConfig
      {
        PortBindings = converter.PortBindings,
        Mounts = converter.Mounts,
      };

      var createParameters = new CreateContainerParameters
      {
        Image = configuration.Image.FullName,
        Name = configuration.Name,
        WorkingDir = configuration.WorkingDirectory,
        Entrypoint = converter.Entrypoint,
        Cmd = converter.Command,
        Env = converter.Environments,
        Labels = converter.Labels,
        ExposedPorts = converter.ExposedPorts,
        HostConfig = hostConfig,
      };

      var id = (await this.Docker.Containers.CreateContainerAsync(createParameters, ct)
        .ConfigureAwait(false)).ID;

      Logger.LogInformation("Container {id} created", id);

      return id;
    }
  }
}
