namespace DotNet.Testcontainers.Client
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using DotNet.Testcontainers.Images.Archives;
  using DotNet.Testcontainers.Images.Configurations;
  using DotNet.Testcontainers.Internals.Mappers;
  using DotNet.Testcontainers.Services;

  /// <summary>
  /// This class represents a Docker API client and manages Docker resources.
  /// </summary>
  internal class TestcontainersClient : DockerApiClient, ITestcontainersClient
  {
    private static readonly Lazy<ITestcontainersClient> TestcontainersClientLazy = new Lazy<ITestcontainersClient>(() => new TestcontainersClient());

    private TestcontainersClient()
    {
      AppDomain.CurrentDomain.ProcessExit += (sender, args) => PurgeOrphanedContainers();
      Console.CancelKeyPress += (sender, args) => PurgeOrphanedContainers();
    }

    internal static ITestcontainersClient Instance { get; } = TestcontainersClientLazy.Value;

    private static void PurgeOrphanedContainers()
    {
      var args = string.Join(" ", TestcontainersRegistryService.GetRegisteredContainers());

      if (string.IsNullOrEmpty(args))
      {
        return;
      }

      new Process { StartInfo = { FileName = "docker", Arguments = $"rm --force {args}" } }.Start();
    }

    public async Task StartAsync(string id, CancellationToken cancellationToken = default)
    {
      if (await DockerApiClientContainer.Instance.ExistsWithIdAsync(id))
      {
        await Docker.Containers.StartContainerAsync(id, new ContainerStartParameters(), cancellationToken);
      }
    }

    public async Task StopAsync(string id, CancellationToken cancellationToken = default)
    {
      if (await DockerApiClientContainer.Instance.ExistsWithIdAsync(id))
      {
        await Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }, cancellationToken);
      }
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
      if (await DockerApiClientContainer.Instance.ExistsWithIdAsync(id))
      {
        await Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true }, cancellationToken);
      }

      TestcontainersRegistryService.Unregister(id);
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken cancellationToken = default)
    {
      if (outputConsumer is null || outputConsumer is OutputConsumerNull)
      {
        return;
      }

      var attachParameters = new ContainerAttachParameters
      {
        Stdout = true,
        Stderr = true,
        Stream = true,
      };

      var stream = await Docker.Containers.AttachContainerAsync(id, false, attachParameters, cancellationToken);

      _ = stream.CopyOutputToAsync(Stream.Null, outputConsumer.Stdout, outputConsumer.Stderr, cancellationToken);
    }

    public async Task<long> ExecAsync(string id, params string[] command)
    {
      if (command is null)
      {
        throw new ArgumentException($"{nameof(command)} cannot be null.");
      }

      var exitCode = long.MinValue;

      var created = await Docker.Containers.ExecCreateContainerAsync(id, new ContainerExecCreateParameters { Cmd = command });

      await Docker.Containers.StartContainerExecAsync(created.ID);

      await WaitStrategy.WaitWhile(async () =>
      {
        var response = await Docker.Containers.InspectContainerExecAsync(created.ID);
        exitCode = response.ExitCode;
        return response.Running;
      });

      return exitCode;
    }

    public async Task<string> BuildAsync(ImageFromDockerfileConfiguration config, CancellationToken cancellationToken = default)
    {
      var dockerFileArchive = new DockerfileArchive(config.DockerfileDirectory);

      var imageExists = await DockerApiClientImage.Instance.ExistsWithNameAsync(config.Image);

      if (imageExists && config.DeleteIfExists)
      {
        await Docker.Images.DeleteImageAsync(config.Image, new ImageDeleteParameters { Force = true }, cancellationToken);
      }

      using (var stream = new FileStream(dockerFileArchive.Tar(), FileMode.Open))
      {
        using (var builtImage = await Docker.Images.BuildImageFromDockerfileAsync(stream, new ImageBuildParameters { Dockerfile = "Dockerfile", Tags = new[] { config.Image } }, cancellationToken))
        {
          // New Docker image built, ready to use.
        }
      }

      return config.Image;
    }

    public async Task<string> RunAsync(TestcontainersConfiguration config, CancellationToken cancellationToken = default)
    {
      var image = config.Container.Image;

      var imageExistsTask = DockerApiClientImage.Instance.ExistsWithNameAsync(image);

      var pullImageTask = Task.CompletedTask;

      if (!await imageExistsTask)
      {
        pullImageTask = Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance, cancellationToken);
      }

      var name = config.Container.Name;

      var workingDir = config.Container.WorkingDirectory;

      var converter = new TestcontainersConfigurationConverter(config);

      var entrypoint = converter.Entrypoint;

      var cmd = converter.Command;

      var env = converter.Environments;

      var labels = converter.Labels;

      var exposedPorts = converter.ExposedPorts;

      var portBindings = converter.PortBindings;

      var mounts = converter.Mounts;

      var hostConfig = new HostConfig
      {
        PortBindings = portBindings,
        Mounts = mounts,
      };

      var createParameters = new CreateContainerParameters
      {
        Image = image,
        Name = name,
        WorkingDir = workingDir,
        Entrypoint = entrypoint,
        Env = env,
        Labels = labels,
        Cmd = cmd,
        ExposedPorts = exposedPorts,
        HostConfig = hostConfig,
      };

      await pullImageTask;

      var id = (await Docker.Containers.CreateContainerAsync(createParameters, cancellationToken)).ID;

      TestcontainersRegistryService.Register(id, config.CleanUp);

      return id;
    }
  }
}
