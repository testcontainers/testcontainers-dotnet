namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Core.Mapper;
  using DotNet.Testcontainers.Core.Mapper.Converters;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Diagnostics;

  internal class TestcontainersClient : DockerApiClient, ITestcontainersClient
  {
    private static readonly Lazy<ITestcontainersClient> Testcontainers = new Lazy<ITestcontainersClient>(() => new TestcontainersClient());

    private static readonly ConverterFactory ConverterFactory = new ConverterFactory();

    private static readonly GenericConverter GenericConverter = new GenericConverter(ConverterFactory);

    private static readonly object IsImageAvailableLock = new object();

    static TestcontainersClient()
    {
      ConverterFactory.Register<IReadOnlyCollection<string>, IList<string>>(
        () => new ConvertList());

      ConverterFactory.Register<IReadOnlyDictionary<string, string>, IList<string>>(
        () => new ConvertList());

      ConverterFactory.Register<IReadOnlyDictionary<string, string>, IDictionary<string, string>>(
        () => new ConvertDictionary());

      ConverterFactory.Register<IReadOnlyDictionary<string, string>, IDictionary<string, EmptyStruct>>(
        () => new ConvertExposedPort(), "ExposedPorts");

      ConverterFactory.Register<IReadOnlyDictionary<string, string>, IDictionary<string, IList<PortBinding>>>(
        () => new ConvertPortBinding(), "PortBindings");

      ConverterFactory.Register<IReadOnlyDictionary<string, string>, IList<Mount>>(
        () => new ConvertMount(), "Mounts");
    }

    internal static ITestcontainersClient Instance
    {
      get
      {
        return Testcontainers.Value;
      }
    }

    public void Start(string id)
    {
      if (MetaDataClientContainers.Instance.ExistsWithId(id))
      {
        Docker.Containers.StartContainerAsync(id, new ContainerStartParameters { }).Wait();
      }
    }

    public void Stop(string id)
    {
      if (MetaDataClientContainers.Instance.ExistsWithId(id))
      {
        Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }).Wait();
      }
    }

    public void Remove(string id)
    {
      if (MetaDataClientContainers.Instance.ExistsWithId(id))
      {
        Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true }).Wait();
      }
    }

    public string Run(TestcontainersConfiguration configuration)
    {
      var image = configuration.Container.Image;

      var name = configuration.Container.Name;

      lock (IsImageAvailableLock)
      {
        if (!MetaDataClientImages.Instance.ExistsWithName(image))
        {
          Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance).Wait();
        }
      }

      var cmd = GenericConverter.Convert<IReadOnlyCollection<string>,
        IList<string>>(configuration.Container.Command);

      var env = GenericConverter.Convert<IReadOnlyDictionary<string, string>,
        IList<string>>(configuration.Container.Environments);

      var labels = GenericConverter.Convert<IReadOnlyDictionary<string, string>,
        IDictionary<string, string>>(configuration.Container.Labels);

      var exposedPorts = GenericConverter.Convert<IReadOnlyDictionary<string, string>,
        IDictionary<string, EmptyStruct>>(configuration.Container.ExposedPorts, "ExposedPorts");

      var portBindings = GenericConverter.Convert<IReadOnlyDictionary<string, string>,
        IDictionary<string, IList<PortBinding>>>(configuration.Host.PortBindings, "PortBindings");

      var mounts = GenericConverter.Convert<IReadOnlyDictionary<string, string>,
        IList<Mount>>(configuration.Host.Mounts, "Mounts");

      var hostConfig = new HostConfig
      {
        PortBindings = portBindings,
        Mounts = mounts,
      };

      return Docker.Containers.CreateContainerAsync(new CreateContainerParameters
      {
        Image = image,
        Name = name,
        Env = env,
        Labels = labels,
        Cmd = cmd,
        ExposedPorts = exposedPorts,
        HostConfig = hostConfig,
      }).Result.ID;
    }
  }
}
