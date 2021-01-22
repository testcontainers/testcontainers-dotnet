namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Network;
  using JetBrains.Annotations;

  /// <summary>
  /// A Testcontainer configuration.
  /// </summary>
  public interface ITestcontainersConfiguration
  {
    /// <summary>
    /// Gets a value indicating whether the Testcontainer is removed by the Docker daemon or not.
    /// </summary>
    [CanBeNull]
    bool? AutoRemove { get; }

    /// <summary>
    /// Gets a value indicating whether the Testcontainer has extended privileges or not.
    /// </summary>
    [CanBeNull]
    bool? Privileged { get; }

    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// Gets the Docker registry authentication configuration.
    /// </summary>
    [NotNull]
    IDockerRegistryAuthenticationConfiguration DockerRegistryAuthConfig { get; }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    [NotNull]
    IDockerImage Image { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [CanBeNull]
    string Name { get; }

    /// <summary>
    /// Gets the hostname.
    /// </summary>
    [CanBeNull]
    string Hostname { get; }

    /// <summary>
    /// Gets the working directory.
    /// </summary>
    [CanBeNull]
    string WorkingDirectory { get; }

    /// <summary>
    /// Gets the entrypoint.
    /// </summary>
    [CanBeNull]
    IEnumerable<string> Entrypoint { get; }

    /// <summary>
    /// Gets the command.
    /// </summary>
    [CanBeNull]
    IEnumerable<string> Command { get; }

    /// <summary>
    /// Gets a list of environment variables.
    /// </summary>
    [CanBeNull]
    IReadOnlyDictionary<string, string> Environments { get; }

    /// <summary>
    /// Gets a list of labels.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> Labels { get; }

    /// <summary>
    /// Gets a list of exposed ports.
    /// </summary>
    [CanBeNull]
    IReadOnlyDictionary<string, string> ExposedPorts { get; }

    /// <summary>
    /// Gets a list of port bindings.
    /// </summary>
    [CanBeNull]
    IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <summary>
    /// Gets a list of volumes.
    /// </summary>
    [CanBeNull]
    IEnumerable<IMount> Mounts { get; }

    /// <summary>
    /// Gets a list of networks.
    /// </summary>
    [CanBeNull]
    IEnumerable<IDockerNetwork> Networks { get; }

    /// <summary>
    /// Gets the output consumer.
    /// </summary>
    [NotNull]
    IOutputConsumer OutputConsumer { get; }

    /// <summary>
    /// Gets the wait strategies.
    /// </summary>
    [NotNull]
    IEnumerable<IWaitUntil> WaitStrategies { get; }

    /// <summary>
    /// Gets the startup callback.
    /// </summary>
    /// <remarks>
    /// This callback will be executed after starting the container, but before executing the wait strategies.
    /// </remarks>
    [NotNull]
    Func<ITestcontainersContainer, CancellationToken, Task> StartupCallback { get; }
  }
}
