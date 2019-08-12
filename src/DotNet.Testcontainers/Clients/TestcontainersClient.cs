namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Core;
  using DotNet.Testcontainers.Core.Mapper;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Core.Wait;
  using DotNet.Testcontainers.Diagnostics;

  internal class TestcontainersClient : DockerApiClient, ITestcontainersClient
  {
    private static readonly Lazy<ITestcontainersClient> Testcontainers = new Lazy<ITestcontainersClient>(() => new TestcontainersClient());

    private TestcontainersClient()
    {
    }

    internal static ITestcontainersClient Instance { get; } = Testcontainers.Value;

    public async Task StartAsync(string id, CancellationToken cancellationToken = default)
    {
      if (await MetaDataClientContainers.Instance.ExistsWithIdAsync(id))
      {
        await Docker.Containers.StartContainerAsync(id, new ContainerStartParameters { }, cancellationToken);
      }
    }

    public async Task StopAsync(string id, CancellationToken cancellationToken = default)
    {
      if (await MetaDataClientContainers.Instance.ExistsWithIdAsync(id))
      {
        await Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }, cancellationToken);
      }
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
      if (await MetaDataClientContainers.Instance.ExistsWithIdAsync(id))
      {
        await Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true }, cancellationToken);
      }
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken cancellationToken = default)
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

      var stream = await Docker.Containers.AttachContainerAsync(id, false, attachParameters, cancellationToken);

      _ = stream.CopyOutputToAsync(Stream.Null, outputConsumer.Stdout, outputConsumer.Stderr, cancellationToken);
    }

    public async Task ExecAsync(string id, params string[] command)
    {
      if (command is null)
      {
        return;
      }

      var created = await Docker.Containers.ExecCreateContainerAsync(id, new ContainerExecCreateParameters { Cmd = command });

      await Docker.Containers.StartContainerExecAsync(created.ID);

      await WaitStrategy.WaitWhile(async () => (await Docker.Containers.InspectContainerExecAsync(created.ID)).Running);
    }

    public async Task<string> BuildAsync(ImageFromDockerfileConfiguration config, CancellationToken cancellationToken = default)
    {
      var dockerFileArchive = new DockerfileArchive(config.DockerfileDirectory);

      await MetaDataClientImages.Instance.ExistsWithNameAsync(config.Image).ContinueWith(async imageExists =>
      {
        if (!await imageExists && config.DeleteIfExists)
        {
          await Docker.Images.DeleteImageAsync(config.Image, new ImageDeleteParameters { Force = true }, cancellationToken);
        }
      });

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

      var pullImageTask = MetaDataClientImages.Instance.ExistsWithNameAsync(image).ContinueWith(async imageExists =>
      {
        if (!await imageExists)
        {
          // await ... does not work here, the image will not be pulled.
          Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance, cancellationToken).GetAwaiter().GetResult();
        }
      });

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

      return (await Docker.Containers.CreateContainerAsync(createParameters, cancellationToken)).ID;
    }
  }
}
