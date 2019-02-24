namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Core.Mapper;
  using DotNet.Testcontainers.Core.Mapper.Converters;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Diagnostics;
  using static LanguageExt.Prelude;

  internal class TestcontainersClient : DockerApiClient, ITestcontainersClient
  {
    private static readonly Lazy<ITestcontainersClient> Testcontainers = new Lazy<ITestcontainersClient>(() => new TestcontainersClient());

    private static readonly ConverterFactory ConverterFactory = new ConverterFactory();

    private static readonly GenericConverter GenericConverter = new GenericConverter(ConverterFactory);

    static TestcontainersClient()
    {
      ConverterFactory.Register<IReadOnlyCollection<string>, IList<string>>(
        () => new ConvertList());

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

    public bool ExistImageById(string id)
    {
      return Optional(id).Match(
        Some: value => notnull(MetaDataClientImages.Instance.ById(value)),
        None: () => false);
    }

    public bool ExistImageByName(string name)
    {
      return Optional(name).Match(
        Some: value => notnull(MetaDataClientImages.Instance.ByName(value)),
        None: () => false);
    }

    public bool ExistContainerById(string id)
    {
      return Optional(id).Match(
        Some: value => notnull(MetaDataClientContainers.Instance.ById(value)),
        None: () => false);
    }

    public bool ExistContainerByName(string name)
    {
      return Optional(name).Match(
        Some: value => notnull(MetaDataClientContainers.Instance.ByName(value)),
        None: () => false);
    }

    public string FindImageNameById(string id)
    {
      return Optional(id).Match(
        Some: value => MetaDataClientImages.Instance.ById(value).RepoTags.FirstOrDefault(),
        None: () => string.Empty);
    }

    public string FindImageNameByName(string name)
    {
      return Optional(name).Match(
        Some: value => MetaDataClientImages.Instance.ByName(value).RepoTags.FirstOrDefault(),
        None: () => string.Empty);
    }

    public string FindContainerNameById(string id)
    {
      return Optional(id).Match(
        Some: value => MetaDataClientContainers.Instance.ById(value).Names.FirstOrDefault(),
        None: () => string.Empty);
    }

    public string FindContainerNameByName(string name)
    {
      return Optional(name).Match(
        Some: value => MetaDataClientContainers.Instance.ByName(value).Names.FirstOrDefault(),
        None: () => string.Empty);
    }

    public void Start(string id)
    {
      if (this.ExistContainerById(id))
      {
        Docker.Containers.StartContainerAsync(id, new ContainerStartParameters { }).Wait();
      }
    }

    public void Stop(string id)
    {
      if (this.ExistContainerById(id))
      {
        Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }).Wait();
      }
    }

    public void Remove(string id)
    {
      if (this.ExistContainerById(id))
      {
        Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true }).Wait();
      }
    }

    public string Run(TestcontainersConfiguration configuration)
    {
      var image = configuration.Container.Image;

      var name = configuration.Container.Name;

      if (!this.ExistImageByName(image))
      {
        Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance).Wait();
      }

      var cmd = ConverterFactory.Get<IReadOnlyCollection<string>,
        IList<string>>().Convert(configuration.Container.Command);

      var exposedPorts = ConverterFactory.Get<IReadOnlyDictionary<string, string>,
        IDictionary<string, EmptyStruct>>("ExposedPorts").Convert(configuration.Container.ExposedPorts);

      var portBindings = ConverterFactory.Get<IReadOnlyDictionary<string, string>,
        IDictionary<string, IList<PortBinding>>>("PortBindings").Convert(configuration.Host.PortBindings);

      var mounts = ConverterFactory.Get<IReadOnlyDictionary<string, string>,
        IList<Mount>>("Mounts").Convert(configuration.Host.Mounts);

      var hostConfig = new HostConfig
      {
        PortBindings = portBindings,
        Mounts = mounts,
      };

      return Docker.Containers.CreateContainerAsync(new CreateContainerParameters
      {
        Image = image,
        Name = name,
        Cmd = cmd,
        HostConfig = hostConfig,
      }).Result.ID;
    }
  }
}
