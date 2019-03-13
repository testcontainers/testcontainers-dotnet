namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Core;
  using DotNet.Testcontainers.Core.Mapper;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Diagnostics;

  internal class TestcontainersClient : DockerApiClient, ITestcontainersClient
  {
    private static readonly Lazy<ITestcontainersClient> Testcontainers = new Lazy<ITestcontainersClient>(() => new TestcontainersClient());

    internal static ITestcontainersClient Instance
    {
      get
      {
        return Testcontainers.Value;
      }
    }

    public async Task StartAsync(string id)
    {
      await MetaDataClientContainers.Instance.ExistsWithIdAsync(id).ContinueWith(async containerExists =>
      {
        if (await containerExists)
        {
          await Docker.Containers.StartContainerAsync(id, new ContainerStartParameters { });
        }
      });
    }

    public async Task StopAsync(string id)
    {
      await MetaDataClientContainers.Instance.ExistsWithIdAsync(id).ContinueWith(async containerExists =>
      {
        if (await containerExists)
        {
          await Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 });
        }
      });
    }

    public async Task RemoveAsync(string id)
    {
      await MetaDataClientContainers.Instance.ExistsWithIdAsync(id).ContinueWith(async containerExists =>
      {
        if (await containerExists)
        {
          await Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true });
        }
      });
    }

    public async Task AttachAsync(string id, IOutputConsumer outputConsumer)
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

      var stream = await Docker.Containers.AttachContainerAsync(id, false, attachParameters);

      await stream.CopyOutputToAsync(null, outputConsumer.Stdout, outputConsumer.Stderr, default(CancellationToken));
    }

    public async Task ExecAsync(string id, params string[] command)
    {
      var created = await Docker.Containers.ExecCreateContainerAsync(id, new ContainerExecCreateParameters
      {
        Cmd = command,
      });

      var startExecTask = Docker.Containers.StartContainerExecAsync(created.ID);

      var commandFinishedTask = WaitStrategy.WaitWhile(async () =>
      {
        return (await Docker.Containers.InspectContainerExecAsync(created.ID)).Running;
      });

      await Task.WhenAll(startExecTask, commandFinishedTask);
    }

    public async Task<string> RunAsync(TestcontainersConfiguration config)
    {
      var image = config.Container.Image;

      var pullImageTask = MetaDataClientImages.Instance.ExistsWithNameAsync(image).ContinueWith(async imageExists =>
      {
        if (!await imageExists)
        {
          // await ... does not work here, the image will not be pulled.
          Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance).GetAwaiter().GetResult();
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

      return (await Docker.Containers.CreateContainerAsync(createParameters)).ID;
    }
  }
}
