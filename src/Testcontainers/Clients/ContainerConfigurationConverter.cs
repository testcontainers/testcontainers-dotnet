namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;

  internal sealed class ContainerConfigurationConverter
  {
    private const string UdpPortSuffix = "/udp";

    private const string TcpPortSuffix = "/tcp";

    private const string SctpPortSuffix = "/sctp";

    public ContainerConfigurationConverter(IContainerConfiguration configuration)
    {
      Entrypoint = new ToCollection().Convert(configuration.Entrypoint)?.ToList();
      Command = new ToCollection().Convert(configuration.Command)?.ToList();
      ExtraHosts = new ToCollection().Convert(configuration.ExtraHosts)?.ToList();
      Environments = new ToMappedList().Convert(configuration.Environments)?.ToList();
      Labels = new ToDictionary().Convert(configuration.Labels)?.ToDictionary(item => item.Key, item => item.Value);
      ExposedPorts = new ToExposedPorts().Convert(configuration.ExposedPorts)?.ToDictionary(item => item.Key, item => item.Value);
      PortBindings = new ToPortBindings().Convert(configuration.PortBindings)?.ToDictionary(item => item.Key, item => item.Value);
      Mounts = new ToMounts().Convert(configuration.Mounts)?.ToList();
      Networks = new ToNetworks(configuration).Convert(configuration.Networks)?.ToDictionary(item => item.Key, item => item.Value);
    }

    public IList<string> Entrypoint { get; }

    public IList<string> Command { get; }

    public IList<string> ExtraHosts { get; }

    public IList<string> Environments { get; }

    public IDictionary<string, string> Labels { get; }

    public IDictionary<string, EmptyStruct> ExposedPorts { get; }

    public IDictionary<string, IList<PortBinding>> PortBindings { get; }

    public IList<Mount> Mounts { get; }

    public IDictionary<string, EndpointSettings> Networks { get; }

    private static string GetQualifiedPort(string containerPort)
    {
      return Array.Exists(new[] { UdpPortSuffix, TcpPortSuffix, SctpPortSuffix }, portSuffix => containerPort.EndsWith(portSuffix, StringComparison.OrdinalIgnoreCase)) ? containerPort.ToLowerInvariant() : containerPort + TcpPortSuffix;
    }

    private sealed class ToCollection : CollectionConverter<string, string>
    {
      public ToCollection()
        : base(nameof(ToCollection))
      {
      }

      public override IEnumerable<string> Convert([CanBeNull] IEnumerable<string> source)
      {
        return source;
      }
    }

    private sealed class ToMounts : CollectionConverter<IMount, Mount>
    {
      public ToMounts()
        : base(nameof(ToMounts))
      {
      }

      public override IEnumerable<Mount> Convert([CanBeNull] IEnumerable<IMount> source)
      {
        return source?.Select(mount =>
        {
          var readOnly = AccessMode.ReadOnly.Equals(mount.AccessMode);
          return new Mount { Type = mount.Type.Type, Source = mount.Source, Target = mount.Target, ReadOnly = readOnly };
        });
      }
    }

    private sealed class ToNetworks : CollectionConverter<INetwork, KeyValuePair<string, EndpointSettings>>
    {
      private readonly IContainerConfiguration _configuration;

      public ToNetworks(IContainerConfiguration configuration)
        : base(nameof(ToNetworks))
      {
        _configuration = configuration;
      }

      public override IEnumerable<KeyValuePair<string, EndpointSettings>> Convert([CanBeNull] IEnumerable<INetwork> source)
      {
        return source?.Select(network => new KeyValuePair<string, EndpointSettings>(network.Name, new EndpointSettings { Aliases = _configuration.NetworkAliases?.ToList() }));
      }
    }

    private sealed class ToMappedList : DictionaryConverter<IEnumerable<string>>
    {
      public ToMappedList()
        : base(nameof(ToMappedList))
      {
      }

      public override IEnumerable<string> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(item => string.Join("=", item.Key, item.Value));
      }
    }

    private sealed class ToDictionary : DictionaryConverter<IEnumerable<KeyValuePair<string, string>>>
    {
      public ToDictionary()
        : base(nameof(ToDictionary))
      {
      }

      public override IEnumerable<KeyValuePair<string, string>> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        return source;
      }
    }

    private sealed class ToExposedPorts : DictionaryConverter<IEnumerable<KeyValuePair<string, EmptyStruct>>>
    {
      public ToExposedPorts()
        : base(nameof(ToExposedPorts))
      {
      }

      public override IEnumerable<KeyValuePair<string, EmptyStruct>> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(exposedPort => new KeyValuePair<string, EmptyStruct>(
          GetQualifiedPort(exposedPort.Key), default));
      }
    }

    private sealed class ToPortBindings : DictionaryConverter<IEnumerable<KeyValuePair<string, IList<PortBinding>>>>
    {
      public ToPortBindings()
        : base(nameof(ToPortBindings))
      {
      }

      public override IEnumerable<KeyValuePair<string, IList<PortBinding>>> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        // https://github.com/moby/moby/pull/41805#issuecomment-893349240.
        return source?.Select(portBinding => new KeyValuePair<string, IList<PortBinding>>(
          GetQualifiedPort(portBinding.Key), new[] { new PortBinding { HostIP = IPAddress.Any.ToString(), HostPort = portBinding.Value } }));
      }
    }
  }
}
