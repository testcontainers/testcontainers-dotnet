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

  /// <summary>
  /// A Testcontainer configuration.
  /// </summary>
  public interface ITestcontainersConfiguration : IDockerResourceConfiguration
  {
    /// <summary>
    /// Gets a value indicating whether the Testcontainer is removed by the Docker daemon or not.
    /// </summary>
    bool? AutoRemove { get; }

    /// <summary>
    /// Gets a value indicating whether the Testcontainer has extended privileges or not.
    /// </summary>
    bool? Privileged { get; }

    /// <summary>
    /// Gets the Docker registry authentication configuration.
    /// </summary>
    IDockerRegistryAuthenticationConfiguration DockerRegistryAuthConfig { get; }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    IDockerImage Image { get; }

    /// <summary>
    /// Gets the image pull policy.
    /// </summary>
    Func<ImagesListResponse, bool> ImagePullPolicy { get; }

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
    /// Gets a list of environment variables.
    /// </summary>
    IReadOnlyDictionary<string, string> Environments { get; }

    /// <summary>
    /// Gets a list of exposed ports.
    /// </summary>
    IReadOnlyDictionary<string, string> ExposedPorts { get; }

    /// <summary>
    /// Gets a list of port bindings.
    /// </summary>
    IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <summary>
    /// Gets a list of resource mappings.
    /// </summary>
    IReadOnlyDictionary<string, IResourceMapping> ResourceMappings { get; }

    /// <summary>
    /// Gets a list of mounts.
    /// </summary>
    IEnumerable<IMount> Mounts { get; }

    /// <summary>
    /// Gets a list of networks.
    /// </summary>
    IEnumerable<IDockerNetwork> Networks { get; }

    /// <summary>
    /// Gets a list of network aliases.
    /// </summary>
    IEnumerable<string> NetworkAliases { get; }

    /// <summary>
    /// Gets the output consumer.
    /// </summary>
    IOutputConsumer OutputConsumer { get; }

    /// <summary>
    /// Gets the wait strategies.
    /// </summary>
    IEnumerable<IWaitUntil> WaitStrategies { get; }

    /// <summary>
    /// Gets a list of actions that modifies the <see cref="CreateContainerParameters" /> configuration.
    /// </summary>
    /// <remarks>
    /// The actions modify the <see cref="CreateContainerParameters" /> configuration after the builder configuration has been applied, but before the container starts.
    /// </remarks>
    IReadOnlyList<Action<CreateContainerParameters>> ParameterModifiers { get; }

    /// <summary>
    /// Gets the startup callback.
    /// </summary>
    /// <remarks>
    /// This callback will be executed after starting the container, but before executing the wait strategies.
    /// </remarks>
    Func<ITestcontainersContainer, CancellationToken, Task> StartupCallback { get; }
  }
}
