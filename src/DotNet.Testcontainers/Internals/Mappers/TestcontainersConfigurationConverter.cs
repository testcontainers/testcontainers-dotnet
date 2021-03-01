namespace DotNet.Testcontainers.Internals.Mappers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Docker.DotNet.Models;

  using DotNet.Testcontainers.Containers.Configurations;

  using Mount = Docker.DotNet.Models.Mount;

  internal sealed class TestcontainersConfigurationConverter
  {
    private const string UdpPortSuffix = "/udp";

    private const string TcpPortSuffix = "/tcp";

    private const string SctpPortSuffix = "/sctp";

    public TestcontainersConfigurationConverter(ITestcontainersConfiguration configuration)
    {
      this.Entrypoint = new ToCollection().Convert(configuration.Entrypoint)?.ToArray();
      this.Command = new ToCollection().Convert(configuration.Command)?.ToArray();
      this.Environments = new ToMappedList().Convert(configuration.Environments)?.ToArray();
      this.Labels = new ToDictionary().Convert(configuration.Labels)?.ToDictionary(item => item.Key, item => item.Value);
      this.ExposedPorts = new ToExposedPorts().Convert(configuration.ExposedPorts)?.ToDictionary(item => item.Key, item => item.Value);
      this.PortBindings = new ToPortBindings().Convert(configuration.PortBindings)?.ToDictionary(item => item.Key, item => item.Value);
      this.Mounts = new ToMounts().Convert(configuration.Mounts)?.ToArray();
    }

    public IList<string> Entrypoint { get; }

    public IList<string> Command { get; }

    public IList<string> Environments { get; }

    public IDictionary<string, string> Labels { get; }

    public IDictionary<string, EmptyStruct> ExposedPorts { get; }

    public IDictionary<string, IList<PortBinding>> PortBindings { get; }

    public IList<Mount> Mounts { get; }

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
        return source?.Select(mount => new Mount { Type = "bind", Source = mount.HostPath, Target = mount.ContainerPath, ReadOnly = AccessMode.ReadOnly.Equals(mount.AccessMode) });
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
        return source?.Select(exposedPort => new KeyValuePair<string, EmptyStruct>(
          GetQualifiedPort(exposedPort.Key), default));
      }
    }

    private sealed class ToPortBindings : DictionaryConverter<IEnumerable<KeyValuePair<string, IList<PortBinding>>>>
    {
      public ToPortBindings() : base(nameof(ToPortBindings)) { }

      public override IEnumerable<KeyValuePair<string, IList<PortBinding>>> Convert(IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(portBinding => new KeyValuePair<string, IList<PortBinding>>(
          GetQualifiedPort(portBinding.Key), new[] { new PortBinding { HostPort = "0".Equals(portBinding.Value) ? null : portBinding.Value } }));
      }
    }

    private static string GetQualifiedPort(string containerPort)
    {
      return new[] { UdpPortSuffix, TcpPortSuffix, SctpPortSuffix }
        .Any(portSuffix => containerPort.EndsWith(portSuffix, StringComparison.OrdinalIgnoreCase))
          ? containerPort.ToLowerInvariant()
          : containerPort + TcpPortSuffix;
    }
  }
}
