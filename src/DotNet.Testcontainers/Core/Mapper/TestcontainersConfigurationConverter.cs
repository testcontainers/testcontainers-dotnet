namespace DotNet.Testcontainers.Core.Mapper
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Core.Models;

  internal class TestcontainersConfigurationConverter
  {
    public TestcontainersConfigurationConverter(TestcontainersConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    public IList<string> Entrypoint => new ToList().Convert(this.Configuration.Container.Entrypoint);

    public IList<string> Command => new ToList().Convert(this.Configuration.Container.Command);

    public IList<string> Environments => new ToMappedList().Convert(this.Configuration.Container.Environments);

    public IDictionary<string, string> Labels => new ToDictionary().Convert(this.Configuration.Container.Labels);

    public IDictionary<string, EmptyStruct> ExposedPorts => new ToExposedPorts().Convert(this.Configuration.Container.ExposedPorts);

    public IDictionary<string, IList<PortBinding>> PortBindings => new ToPortBindings().Convert(this.Configuration.Host.PortBindings);

    public IList<Mount> Mounts => new ToMounts().Convert(this.Configuration.Host.Mounts);

    private TestcontainersConfiguration Configuration { get; }

    private class ToList : CollectionConverter<IList<string>>
    {
      public override IList<string> Convert(IReadOnlyCollection<string> source)
      {
        return source.ToList();
      }
    }

    private class ToDictionary : DictionaryConverter<IDictionary<string, string>>
    {
      public override IDictionary<string, string> Convert(IReadOnlyDictionary<string, string> source)
      {
        return source.ToDictionary(item => item.Key, item => item.Value);
      }
    }

    private class ToMappedList : DictionaryConverter<IList<string>>
    {
      public override IList<string> Convert(IReadOnlyDictionary<string, string> source)
      {
        return source.Select(item => $"{item.Key}={item.Value}").ToList();
      }
    }

    private class ToExposedPorts : DictionaryConverter<IDictionary<string, EmptyStruct>>
    {
      public ToExposedPorts() : base("ExposedPorts")
      {
      }

      public override IDictionary<string, EmptyStruct> Convert(IReadOnlyDictionary<string, string> source)
      {
        return source.ToDictionary(exposedPort => $"{exposedPort.Key}/tcp", exposedPort => default(EmptyStruct));
      }
    }

    private class ToPortBindings : DictionaryConverter<IDictionary<string, IList<PortBinding>>>
    {
      public ToPortBindings() : base("PortBindings")
      {
      }

      public override IDictionary<string, IList<PortBinding>> Convert(IReadOnlyDictionary<string, string> source)
      {
        return source.ToDictionary(binding => $"{binding.Value}/tcp", binding => new List<PortBinding> { new PortBinding { HostPort = binding.Key } } as IList<PortBinding>);
      }
    }

    private class ToMounts : DictionaryConverter<IList<Mount>>
    {
      public ToMounts() : base("Mounts")
      {
      }

      public override IList<Mount> Convert(IReadOnlyDictionary<string, string> source)
      {
        return source.Select(mount => new Mount { Source = Path.GetFullPath(mount.Key), Target = mount.Value, Type = "bind" }).ToList();
      }
    }
  }
}
