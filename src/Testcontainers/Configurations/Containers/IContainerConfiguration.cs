namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Networks;
  using JetBrains.Annotations;

  /// <summary>
  /// A container configuration.
  /// </summary>
  [PublicAPI]
  public interface IContainerConfiguration : IResourceConfiguration<CreateContainerParameters>
  {
    /// <summary>
    /// Gets a value indicating whether Docker removes the container after it exits or not.
    /// </summary>
    bool? AutoRemove { get; }

    /// <summary>
    /// Gets a value indicating whether the privileged flag is set or not.
    /// </summary>
    bool? Privileged { get; }

    /// <summary>
    /// Gets the image.
    /// </summary>
    IImage Image { get; }

    /// <summary>
    /// Gets the image pull policy.
    /// </summary>
    Func<ImageInspectResponse, bool> ImagePullPolicy { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the hostname.
    /// </summary>
    string Hostname { get; }

    /// <summary>
    /// Gets the MAC address.
    /// </summary>
    string MacAddress { get; }

    /// <summary>
    /// Gets the working directory.
    /// </summary>
    string WorkingDirectory { get; }

    /// <summary>
    /// Gets the entrypoint.
    /// </summary>
    IEnumerable<string> Entrypoint { get; }

    /// <summary>
    /// Gets the command.
    /// </summary>
    IEnumerable<string> Command { get; }

    /// <summary>
    /// Gets a dictionary of environment variables.
    /// </summary>
    IReadOnlyDictionary<string, string> Environments { get; }

    /// <summary>
    /// Gets a dictionary of exposed ports.
    /// </summary>
    IReadOnlyDictionary<string, string> ExposedPorts { get; }

    /// <summary>
    /// Gets a dictionary of port bindings.
    /// </summary>
    IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <summary>
    /// Gets a list of resource mappings.
    /// </summary>
    IEnumerable<IResourceMapping> ResourceMappings { get; }

    /// <summary>
    /// Gets a list of dependent containers.
    /// </summary>
    IEnumerable<IContainer> Containers { get; }

    /// <summary>
    /// Gets a list of dependent mounts.
    /// </summary>
    IEnumerable<IMount> Mounts { get; }

    /// <summary>
    /// Gets a list of dependent networks.
    /// </summary>
    IEnumerable<INetwork> Networks { get; }

    /// <summary>
    /// Gets a list of network-scoped aliases.
    /// </summary>
    IEnumerable<string> NetworkAliases { get; }

    /// <summary>
    /// Gets a list of extra hosts.
    /// </summary>
    IEnumerable<string> ExtraHosts { get; }

    /// <summary>
    /// Gets the output consumer.
    /// </summary>
    IOutputConsumer OutputConsumer { get; }

    /// <summary>
    /// Gets the wait strategies.
    /// </summary>
    IEnumerable<IWaitUntil> WaitStrategies { get; }

    /// <summary>
    /// Gets the startup callback.
    /// </summary>
    Func<IContainer, CancellationToken, Task> StartupCallback { get; }
  }
}
