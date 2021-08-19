namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerContainerOperations : DockerApiClient, IDockerContainerOperations
  {
    private readonly ILogger logger;

    public DockerContainerOperations(Uri endpoint, ILogger logger)
      : base(endpoint)
    {
      this.logger = logger;
    }

    public async Task<IEnumerable<ContainerListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await this.Docker.Containers.ListContainersAsync(new ContainersListParameters { All = true }, ct)
        .ConfigureAwait(false)).ToArray();
    }

    public async Task<IEnumerable<ContainerListResponse>> GetOrphanedObjects(CancellationToken ct = default)
    {
      var filters = new FilterByProperty { { "label", $"{TestcontainersClient.TestcontainersCleanUpLabel}={bool.TrueString}" }, { "status", "exited" } };
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
      var filters = new FilterByProperty { { property, value } };
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
      this.logger.LogInformation("Starting container {id}", id);
      return this.Docker.Containers.StartContainerAsync(id, new ContainerStartParameters(), ct);
    }

    public Task StopAsync(string id, CancellationToken ct = default)
    {
      this.logger.LogInformation("Stopping container {id}", id);
      return this.Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }, ct);
    }

    public Task RemoveAsync(string id, CancellationToken ct = default)
    {
      this.logger.LogInformation("Removing container {id}", id);
      return this.Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true, RemoveVolumes = true }, ct);
    }

    public Task ExtractArchiveToContainerAsync(string id, string path, Stream tarStream, CancellationToken ct = default)
    {
      this.logger.LogInformation("Copying tar stream to {path} at container {id}", path, id);
      return this.Docker.Containers.ExtractArchiveToContainerAsync(id, new ContainerPathStatParameters { Path = path, AllowOverwriteDirWithFile = false }, tarStream, ct);
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default)
    {
      this.logger.LogInformation("Attaching {outputConsumer} at container {id}", outputConsumer.GetType(), id);

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

    public async Task<ExecResult> ExecAsync(string id, IList<string> command, CancellationToken ct = default)
    {
      this.logger.LogInformation("Executing {command} at container {id}", command, id);

      var execCreateParameters = new ContainerExecCreateParameters
      {
        Cmd = command,
        AttachStdout = true,
        AttachStderr = true,
      };

      var execCreateResponse = await this.Docker.Exec.ExecCreateContainerAsync(id, execCreateParameters, ct)
        .ConfigureAwait(false);

      var stdOutAndErrStream = await this.Docker.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false, ct)
        .ConfigureAwait(false);

      for (ContainerExecInspectResponse response; (response = await this.Docker.Exec.InspectContainerExecAsync(execCreateResponse.ID, ct)
        .ConfigureAwait(false)) != null;)
      {
        if (response.Running)
        {
          continue;
        }

        var (stdout, stderr) = await stdOutAndErrStream.ReadOutputToEndAsync(ct)
          .ConfigureAwait(false);

        return new ExecResult(stdout, stderr, response.ExitCode);
      }

      return ExecResult.Failure;
    }

    public async Task<string> RunAsync(ITestcontainersConfiguration configuration, CancellationToken ct = default)
    {
      var converter = new TestcontainersConfigurationConverter(configuration);

      var hostConfig = new HostConfig
      {
        PortBindings = converter.PortBindings,
        Mounts = converter.Mounts,
      };

      var networkingConfig = new NetworkingConfig
      {
        EndpointsConfig = converter.Networks,
      };

      var createParameters = new CreateContainerParameters
      {
        Image = configuration.Image.FullName,
        Name = configuration.Name,
        Hostname = configuration.Hostname,
        WorkingDir = configuration.WorkingDirectory,
        Entrypoint = converter.Entrypoint,
        Cmd = converter.Command,
        Env = converter.Environments,
        Labels = converter.Labels,
        ExposedPorts = converter.ExposedPorts,
        HostConfig = hostConfig,
        NetworkingConfig = networkingConfig,
      };

      var id = (await this.Docker.Containers.CreateContainerAsync(createParameters, ct)
        .ConfigureAwait(false)).ID;

      this.logger.LogInformation("Container {id} created", id);

      return id;
    }
  }
}
