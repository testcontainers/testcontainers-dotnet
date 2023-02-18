namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
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

    public DockerContainerOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig)
    {
      this.logger = logger;
    }

    public async Task<IEnumerable<ContainerListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await this.Docker.Containers.ListContainersAsync(new ContainersListParameters { All = true }, ct)
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

    public async Task<long> GetExitCodeAsync(string id, CancellationToken ct = default)
    {
      return (await this.Docker.Containers.WaitContainerAsync(id, ct)
        .ConfigureAwait(false)).StatusCode;
    }

    public async Task<(string Stdout, string Stderr)> GetLogsAsync(string id, TimeSpan since, TimeSpan until, bool timestampsEnabled = true, CancellationToken ct = default)
    {
      var logsParameters = new ContainerLogsParameters
      {
        ShowStdout = true,
        ShowStderr = true,
        Since = since.TotalSeconds.ToString("0", CultureInfo.InvariantCulture),
        Until = until.TotalSeconds.ToString("0", CultureInfo.InvariantCulture),
        Timestamps = timestampsEnabled,
      };

      using (var stdOutAndErrStream = await this.Docker.Containers.GetContainerLogsAsync(id, false, logsParameters, ct)
        .ConfigureAwait(false))
      {
        return await stdOutAndErrStream.ReadOutputToEndAsync(ct)
          .ConfigureAwait(false);
      }
    }

    public Task StartAsync(string id, CancellationToken ct = default)
    {
      this.logger.StartDockerContainer(id);
      return this.Docker.Containers.StartContainerAsync(id, new ContainerStartParameters(), ct);
    }

    public Task StopAsync(string id, CancellationToken ct = default)
    {
      this.logger.StopDockerContainer(id);
      return this.Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }, ct);
    }

    public Task RemoveAsync(string id, CancellationToken ct = default)
    {
      this.logger.DeleteDockerContainer(id);
      return this.Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true, RemoveVolumes = true }, ct);
    }

    public Task ExtractArchiveToContainerAsync(string id, string path, Stream tarStream, CancellationToken ct = default)
    {
      this.logger.ExtractArchiveToDockerContainer(id, path);
      return this.Docker.Containers.ExtractArchiveToContainerAsync(id, new ContainerPathStatParameters { Path = path, AllowOverwriteDirWithFile = false }, tarStream, ct);
    }

    public async Task<Stream> GetArchiveFromContainerAsync(string id, string path, CancellationToken ct = default)
    {
      this.logger.GetArchiveFromDockerContainer(id, path);

      var tarResponse = await this.Docker.Containers.GetArchiveFromContainerAsync(id, new GetArchiveFromContainerParameters { Path = path }, false, ct)
        .ConfigureAwait(false);

      return tarResponse.Stream;
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default)
    {
      if (!outputConsumer.Enabled)
      {
        return;
      }

      this.logger.AttachToDockerContainer(id, outputConsumer.GetType());

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
      this.logger.ExecuteCommandInDockerContainer(id, command);

      var execCreateParameters = new ContainerExecCreateParameters
      {
        Cmd = command,
        AttachStdout = true,
        AttachStderr = true,
      };

      var execCreateResponse = await this.Docker.Exec.ExecCreateContainerAsync(id, execCreateParameters, ct)
        .ConfigureAwait(false);

      using (var stdOutAndErrStream = await this.Docker.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false, ct)
        .ConfigureAwait(false))
      {
        var (stdout, stderr) = await stdOutAndErrStream.ReadOutputToEndAsync(ct)
          .ConfigureAwait(false);

        var execInspectResponse = await this.Docker.Exec.InspectContainerExecAsync(execCreateResponse.ID, ct)
          .ConfigureAwait(false);

        return new ExecResult(stdout, stderr, execInspectResponse.ExitCode);
      }
    }

    public async Task<string> RunAsync(IContainerConfiguration configuration, CancellationToken ct = default)
    {
      var converter = new ContainerConfigurationConverter(configuration);

      var hostConfig = new HostConfig
      {
        AutoRemove = configuration.AutoRemove.HasValue && configuration.AutoRemove.Value,
        Privileged = configuration.Privileged.HasValue && configuration.Privileged.Value,
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
        MacAddress = configuration.MacAddress,
        WorkingDir = configuration.WorkingDirectory,
        Entrypoint = converter.Entrypoint,
        Cmd = converter.Command,
        Env = converter.Environments,
        Labels = converter.Labels,
        ExposedPorts = converter.ExposedPorts,
        HostConfig = hostConfig,
        NetworkingConfig = networkingConfig,
      };

      if (configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in configuration.ParameterModifiers)
        {
          parameterModifier(createParameters);
        }
      }

      var createContainerResponse = await this.Docker.Containers.CreateContainerAsync(createParameters, ct)
        .ConfigureAwait(false);

      this.logger.DockerContainerCreated(createContainerResponse.ID);
      return createContainerResponse.ID;
    }

    public Task<ContainerInspectResponse> InspectAsync(string id, CancellationToken ct = default)
    {
      return this.Docker.Containers.InspectContainerAsync(id, ct);
    }
  }
}
