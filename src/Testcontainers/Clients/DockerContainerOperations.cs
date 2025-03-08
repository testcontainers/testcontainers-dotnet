namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerContainerOperations : DockerApiClient, IDockerContainerOperations
  {
    public DockerContainerOperations(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(sessionId, dockerEndpointAuthConfig, logger)
    {
    }

    public async Task<IEnumerable<ContainerListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return await DockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true }, ct)
        .ConfigureAwait(false);
    }

    public async Task<IEnumerable<ContainerListResponse>> GetAllAsync(FilterByProperty filters, CancellationToken ct = default)
    {
      return await DockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true, Filters = filters }, ct)
        .ConfigureAwait(false);
    }

    public async Task<ContainerInspectResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return await DockerClient.Containers.InspectContainerAsync(id, ct)
        .ConfigureAwait(false);
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      try
      {
        _ = await ByIdAsync(id, ct)
          .ConfigureAwait(false);

        return true;
      }
      catch (DockerContainerNotFoundException)
      {
        return false;
      }
    }

    public async Task<long> GetExitCodeAsync(string id, CancellationToken ct = default)
    {
      var response = await DockerClient.Containers.WaitContainerAsync(id, ct)
        .ConfigureAwait(false);

      return response.StatusCode;
    }

    public async Task<(string Stdout, string Stderr)> GetLogsAsync(string id, TimeSpan since, TimeSpan until, bool timestampsEnabled = true, CancellationToken ct = default)
    {
      var logsParameters = new ContainerLogsParameters
      {
        ShowStdout = true,
        ShowStderr = true,
        Since = Math.Max(0, Math.Floor(since.TotalSeconds)).ToString("0", CultureInfo.InvariantCulture),
        Until = Math.Max(0, Math.Floor(until.TotalSeconds)).ToString("0", CultureInfo.InvariantCulture),
        Timestamps = timestampsEnabled,
      };

      using (var stdOutAndErrStream = await DockerClient.Containers.GetContainerLogsAsync(id, false, logsParameters, ct)
        .ConfigureAwait(false))
      {
        return await stdOutAndErrStream.ReadOutputToEndAsync(ct)
          .ConfigureAwait(false);
      }
    }

    public Task StartAsync(string id, CancellationToken ct = default)
    {
      Logger.StartDockerContainer(id);
      return DockerClient.Containers.StartContainerAsync(id, new ContainerStartParameters(), ct);
    }

    public Task StopAsync(string id, CancellationToken ct = default)
    {
      Logger.StopDockerContainer(id);
      return DockerClient.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }, ct);
    }

    public Task PauseAsync(string id, CancellationToken ct = default)
    {
      Logger.PauseDockerContainer(id);
      return DockerClient.Containers.PauseContainerAsync(id, ct);
    }

    public Task UnpauseAsync(string id, CancellationToken ct = default)
    {
      Logger.UnpauseDockerContainer(id);
      return DockerClient.Containers.UnpauseContainerAsync(id, ct);
    }

    public Task RemoveAsync(string id, CancellationToken ct = default)
    {
      Logger.DeleteDockerContainer(id);
      return DockerClient.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true, RemoveVolumes = true }, ct);
    }

    public Task ExtractArchiveToContainerAsync(string id, string path, TarOutputMemoryStream tarStream, CancellationToken ct = default)
    {
      Logger.CopyArchiveToDockerContainer(id, tarStream.ContentLength);
      return DockerClient.Containers.ExtractArchiveToContainerAsync(id, new ContainerPathStatParameters { Path = path }, tarStream, ct);
    }

    public async Task<Stream> GetArchiveFromContainerAsync(string id, string path, CancellationToken ct = default)
    {
      Logger.ReadArchiveFromDockerContainer(id, path);

      var tarResponse = await DockerClient.Containers.GetArchiveFromContainerAsync(id, new GetArchiveFromContainerParameters { Path = path }, false, ct)
        .ConfigureAwait(false);

      return tarResponse.Stream;
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default)
    {
      if (!outputConsumer.Enabled)
      {
        return;
      }

      Logger.AttachToDockerContainer(id, outputConsumer.GetType());

      var attachParameters = new ContainerAttachParameters
      {
        Stdout = true,
        Stderr = true,
        Stream = true,
      };

      var stream = await DockerClient.Containers.AttachContainerAsync(id, false, attachParameters, ct)
        .ConfigureAwait(false);

      _ = stream.CopyOutputToAsync(Stream.Null, outputConsumer.Stdout, outputConsumer.Stderr, ct)
        .ConfigureAwait(false);
    }

    public async Task<ExecResult> ExecAsync(string id, IList<string> command, CancellationToken ct = default)
    {
      Logger.ExecuteCommandInDockerContainer(id, command);

      var execCreateParameters = new ContainerExecCreateParameters
      {
        Cmd = command,
        AttachStdout = true,
        AttachStderr = true,
      };

      var execCreateResponse = await DockerClient.Exec.ExecCreateContainerAsync(id, execCreateParameters, ct)
        .ConfigureAwait(false);

      using (var stdOutAndErrStream = await DockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false, ct)
        .ConfigureAwait(false))
      {
        var (stdout, stderr) = await stdOutAndErrStream.ReadOutputToEndAsync(ct)
          .ConfigureAwait(false);

        var execInspectResponse = await DockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID, ct)
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
        ExtraHosts = converter.ExtraHosts,
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

      if (configuration.Reuse.HasValue && configuration.Reuse.Value)
      {
        createParameters.Labels.Add(TestcontainersClient.TestcontainersReuseHashLabel, configuration.GetReuseHash());
      }

      if (configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in configuration.ParameterModifiers)
        {
          parameterModifier(createParameters);
        }
      }

      var createContainerResponse = await DockerClient.Containers.CreateContainerAsync(createParameters, ct)
        .ConfigureAwait(false);

      Logger.DockerContainerCreated(createContainerResponse.ID);
      return createContainerResponse.ID;
    }
  }
}
