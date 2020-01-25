namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A Testcontainer configuration.
  /// </summary>
  public interface ITestcontainersConfiguration
  {
    /// <summary>
    /// If true, the Testcontainer is removed on finalizing. Otherwise, it is kept.
    /// </summary>
    bool CleanUp { get; }

    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    [NotNull]
    IDockerImage Image { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Gets the working directory.
    /// </summary>
    [NotNull]
    string WorkingDirectory { get; }

    /// <summary>
    /// Gets the entrypoint.
    /// </summary>
    [NotNull]
    IEnumerable<string> Entrypoint { get; }

    /// <summary>
    /// Gets the command.
    /// </summary>
    [NotNull]
    IEnumerable<string> Command { get; }

    /// <summary>
    /// Gets a list of environment variables.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> Environments { get; }

    /// <summary>
    /// Gets a list of labels.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> Labels { get; }

    /// <summary>
    /// Gets a list of exposed ports.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> ExposedPorts { get; }

    /// <summary>
    /// Gets a list of port bindings.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <summary>
    /// Gets a list of volumes.
    /// </summary>
    [NotNull]
    IEnumerable<IBind> Mounts { get; }

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
  }
}
