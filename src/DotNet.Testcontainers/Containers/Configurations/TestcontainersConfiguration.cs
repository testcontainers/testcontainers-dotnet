namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using DotNet.Testcontainers.Images;

  /// <inheritdoc cref="ITestcontainersConfiguration" />
  public readonly struct TestcontainersConfiguration : ITestcontainersConfiguration
  {
#pragma warning disable S107

    public TestcontainersConfiguration(
      Uri endpoint,
      IDockerImage image,
      string name,
      string workingDirectory,
      IEnumerable<string> entrypoint,
      IEnumerable<string> command,
      IReadOnlyDictionary<string, string> environments,
      IReadOnlyDictionary<string, string> labels,
      IReadOnlyDictionary<string, string> exposedPorts,
      IReadOnlyDictionary<string, string> portBindings,
      IEnumerable<IBind> mounts,
      IOutputConsumer outputConsumer,
      IEnumerable<IWaitUntil> waitStrategies,
      bool cleanUp = true)
    {
      this.CleanUp  = cleanUp;
      this.Endpoint  = endpoint;
      this.Image  = image;
      this.Name  = name;
      this.WorkingDirectory  = workingDirectory;
      this.Entrypoint  = entrypoint;
      this.Command  = command;
      this.Environments  = environments;
      this.Labels  = labels;
      this.ExposedPorts  = exposedPorts;
      this.PortBindings  = portBindings;
      this.Mounts  = mounts;
      this.OutputConsumer  = outputConsumer;
      this.WaitStrategies  = waitStrategies;
    }

#pragma warning restore S107

    /// <inheritdoc />
    public bool CleanUp { get; }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public IDockerImage Image { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string WorkingDirectory { get; }

    /// <inheritdoc />
    public IEnumerable<string> Entrypoint { get; }

    /// <inheritdoc />
    public IEnumerable<string> Command { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Environments { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> ExposedPorts { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> PortBindings { get; }

    /// <inheritdoc />
    public IEnumerable<IBind> Mounts { get; }

    /// <inheritdoc />
    public IOutputConsumer OutputConsumer { get; }

    /// <inheritdoc />
    public IEnumerable<IWaitUntil> WaitStrategies { get; }
  }
}
