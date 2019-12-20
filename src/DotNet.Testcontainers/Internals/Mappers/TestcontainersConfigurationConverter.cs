namespace DotNet.Testcontainers.Internals.Mappers
{
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers.Configurations;
  using Mount = Docker.DotNet.Models.Mount;

  internal sealed class TestcontainersConfigurationConverter
  {
    private readonly ITestcontainersConfiguration configuration;

    public TestcontainersConfigurationConverter(ITestcontainersConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public IList<string> Entrypoint => new ToCollection().Convert(this.configuration.Entrypoint)?.ToArray();

    public IList<string> Command => new ToCollection().Convert(this.configuration.Command)?.ToArray();

    public IList<string> Environments => new ToMappedList().Convert(this.configuration.Environments)?.ToArray();

    public IDictionary<string, string> Labels => new ToDictionary().Convert(this.configuration.Labels)?.ToDictionary(item => item.Key, item => item.Value);

    public IDictionary<string, EmptyStruct> ExposedPorts => new ToExposedPorts().Convert(this.configuration.ExposedPorts)?.ToDictionary(item => item.Key, item => item.Value);

    public IDictionary<string, IList<PortBinding>> PortBindings => new ToPortBindings().Convert(this.configuration.PortBindings)?.ToDictionary(item => item.Key, item => item.Value);

    public IList<Mount> Mounts => new ToMounts().Convert(this.configuration.Mounts)?.ToArray();

    private sealed class ToCollection : CollectionConverter<string, string>
    {
      public ToCollection() : base(nameof(ToCollection)) { }

      public override IEnumerable<string> Convert(IEnumerable<string> source)
      {
        return source;
      }
    }

    private sealed class ToMounts : CollectionConverter<IBind, Mount>
    {
      public ToMounts() : base(nameof(ToMounts)) { }

      public override IEnumerable<Mount> Convert(IEnumerable<IBind> source)
      {
        return source?.Select(mount => new Mount
        {
          Type = "bind", Source = mount.HostPath, Target = mount.ContainerPath, ReadOnly = AccessMode.ReadOnly.Equals(mount.AccessMode)
        });
      }
    }

    private sealed class ToMappedList : DictionaryConverter<IEnumerable<string>>
    {
      public ToMappedList() : base(nameof(ToMappedList)) { }

      public override IEnumerable<string> Convert(IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(item => $"{item.Key}={item.Value}");
      }
    }

    private sealed class ToDictionary : DictionaryConverter<IEnumerable<KeyValuePair<string, string>>>
    {
      public ToDictionary() : base(nameof(ToDictionary)) { }

      public override IEnumerable<KeyValuePair<string, string>> Convert(IEnumerable<KeyValuePair<string, string>> source)
      {
        return source;
      }
    }

    private sealed class ToExposedPorts : DictionaryConverter<IEnumerable<KeyValuePair<string, EmptyStruct>>>
    {
      public ToExposedPorts() : base(nameof(ToExposedPorts)) { }

      public override IEnumerable<KeyValuePair<string, EmptyStruct>> Convert(IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(exposedPort => new KeyValuePair<string, EmptyStruct>($"{exposedPort.Key}/tcp", default));
      }
    }

    private sealed class ToPortBindings : DictionaryConverter<IEnumerable<KeyValuePair<string, IList<PortBinding>>>>
    {
      public ToPortBindings() : base(nameof(ToPortBindings)) { }

      public override IEnumerable<KeyValuePair<string, IList<PortBinding>>> Convert(IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(portBinding => new KeyValuePair<string, IList<PortBinding>>($"{portBinding.Value}/tcp", new List<PortBinding> { new PortBinding { HostPort = portBinding.Key } }));
      }
    }
  }
}
