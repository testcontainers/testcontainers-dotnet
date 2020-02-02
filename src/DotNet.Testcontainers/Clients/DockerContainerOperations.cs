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

  internal sealed class DockerContainerOperations : DockerApiClient, IDockerContainerOperations
  {
    public DockerContainerOperations(Uri endpoint) : base(endpoint)
    {
    }

    public async Task<IEnumerable<ContainerListResponse>> GetAllAsync(CancellationToken ct = default)
    {
      return (await this.Docker.Containers.ListContainersAsync(new ContainersListParameters { All = true }, ct)).ToArray();
    }

    public async Task<ContainerListResponse> ByIdAsync(string id, CancellationToken ct = default)
    {
      return await this.ByPropertyAsync("id", id, ct);
    }

    public async Task<ContainerListResponse> ByNameAsync(string name, CancellationToken ct = default)
    {
      return await this.ByPropertyAsync("name", name, ct);
    }

    public async Task<ContainerListResponse> ByPropertyAsync(string property, string value, CancellationToken ct = default)
    {
      var response = this.Docker.Containers.ListContainersAsync(new ContainersListParameters
      {
        All = true,
        Filters = new Dictionary<string, IDictionary<string, bool>>
        {
          {
            property, new Dictionary<string, bool>
            {
              { value, true },
            }
          },
        },
      }, ct);

      return (await response).FirstOrDefault();
    }

    public async Task<bool> ExistsWithIdAsync(string id, CancellationToken ct = default)
    {
      return await this.ByIdAsync(id, ct) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken ct = default)
    {
      return await this.ByNameAsync(name, ct) != null;
    }

    public async Task<long> GetExitCode(string id, CancellationToken ct = default)
    {
      return (await this.Docker.Containers.WaitContainerAsync(id, ct)).StatusCode;
    }

    public Task StartAsync(string id, CancellationToken ct = default)
    {
      return this.Docker.Containers.StartContainerAsync(id, new ContainerStartParameters(), ct);
    }

    public Task StopAsync(string id, CancellationToken ct = default)
    {
      return this.Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }, ct);
    }

    public Task RemoveAsync(string id, CancellationToken ct = default)
    {
      return this.Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true, RemoveVolumes = true }, ct);
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default)
    {
      if (outputConsumer is null)
      {
        return;
      }

      var attachParameters = new ContainerAttachParameters
      {
        Stdout = true,
        Stderr = true,
        Stream = true,
      };

      var stream = await this.Docker.Containers.AttachContainerAsync(id, false, attachParameters, ct);
      _ = stream.CopyOutputToAsync(Stream.Null, outputConsumer.Stdout, outputConsumer.Stderr, ct);
    }

    public async Task<long> ExecAsync(string id, IList<string> command, CancellationToken ct = default)
    {
      var created = await this.Docker.Containers.ExecCreateContainerAsync(id, new ContainerExecCreateParameters { Cmd = command, }, ct);

      await this.Docker.Containers.StartContainerExecAsync(created.ID, ct);

      for (ContainerExecInspectResponse response; (response = await this.Docker.Containers.InspectContainerExecAsync(created.ID, ct)) != null;)
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

      return (await this.Docker.Containers.CreateContainerAsync(createParameters, ct)).ID;
    }
  }
}
