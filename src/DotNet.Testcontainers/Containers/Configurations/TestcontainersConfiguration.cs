namespace DotNet.Testcontainers.Containers.Configurations
{
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.OutputConsumers;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  /// <summary>
  /// This class represents a Testcontainer configuration. Based on this configuration, the desired Testcontainer is created.
  /// </summary>
  public sealed class TestcontainersConfiguration
  {
    private static readonly IOutputConsumer DefaultOutputConsumer = new OutputConsumerNull();

    private static readonly IWaitUntil DefaultWaitStrategy = new WaitUntilContainerIsRunning();

    public ContainerConfiguration Container { get; set; } = new ContainerConfiguration();

    public HostConfiguration Host { get; set; } = new HostConfiguration();

    public bool CleanUp { get; set; } = true;

    public IOutputConsumer OutputConsumer { get; set; } = DefaultOutputConsumer;

    public IWaitUntil WaitStrategy { get; set; } = DefaultWaitStrategy;

    /// <summary>
    /// Merges all existing and new Testcontainer configuration changes. This allows us to reuse previous configured configurations, <seealso cref="TestcontainersBuilder{T}" />.
    /// </summary>
    /// <param name="old">Previous Testcontainer configuration.</param>
    /// <returns>An updated Testcontainer configuration.</returns>
    internal TestcontainersConfiguration Merge(TestcontainersConfiguration old)
    {
      this.Container.Image = Merge(this.Container.Image, old.Container.Image);

      this.Container.Name = Merge(this.Container.Name, old.Container.Name);

      this.Container.WorkingDirectory = Merge(this.Container.WorkingDirectory, old.Container.WorkingDirectory);

      this.Container.Entrypoint = Merge(this.Container.Entrypoint, old.Container.Entrypoint);

      this.Container.Command = Merge(this.Container.Command, old.Container.Command);

      this.Container.Environments = Merge(this.Container.Environments, old.Container.Environments);

      this.Container.ExposedPorts = Merge(this.Container.ExposedPorts, old.Container.ExposedPorts);

      this.Container.Labels = Merge(this.Container.Labels, old.Container.Labels);

      this.Host.PortBindings = Merge(this.Host.PortBindings, old.Host.PortBindings);

      this.Host.Mounts = Merge(this.Host.Mounts, old.Host.Mounts);

      this.CleanUp = this.CleanUp && old.CleanUp;

      this.OutputConsumer = Merge(this.OutputConsumer, old.OutputConsumer, DefaultOutputConsumer);

      this.WaitStrategy = Merge(this.WaitStrategy, old.WaitStrategy, DefaultWaitStrategy);

      return this;
    }

    /// <summary>
    /// Returns the changed Testcontainer configuration object. If there is no change, the previous Testcontainer configuration object is returned.
    /// </summary>
    /// <param name="myself">Changed Testcontainer configuration object.</param>
    /// <param name="old">Previous Testcontainer configuration object.</param>
    /// <param name="defaultConfiguration">Default Testcontainer configuration.</param>
    /// <typeparam name="T">Any class.</typeparam>
    /// <returns>Changed Testcontainer configuration object. If there is no change, the previous Testcontainer configuration object.</returns>
    private static T Merge<T>(T myself, T old, T defaultConfiguration = null)
      where T : class
    {
      return myself == null || myself.Equals(defaultConfiguration) ? old : myself;
    }

    /// <summary>
    /// Merges all existing and new Testcontainer configuration changes. If there are no changes, the previous Testcontainer configurations are returned.
    /// </summary>
    /// <param name="myself">Changed Testcontainer configuration.</param>
    /// <param name="old">Previous Testcontainer configuration.</param>
    /// <typeparam name="T">Type of IReadOnlyCollection{T}.</typeparam>
    /// <returns>An updated Testcontainer configuration.</returns>
    private static IReadOnlyCollection<T> Merge<T>(IReadOnlyCollection<T> myself, IReadOnlyCollection<T> old)
      where T : class
    {
      if (myself == null || old == null)
      {
        return myself ?? old;
      }
      else
      {
        return myself.Concat(old).ToList();
      }
    }

    /// <summary>
    /// Merges all existing and new Testcontainer configuration changes. If there are no changes, the previous Testcontainer configurations are returned.
    /// </summary>
    /// <param name="myself">Changed Testcontainer configuration.</param>
    /// <param name="old">Previous Testcontainer configuration.</param>
    /// <typeparam name="T">Type of IReadOnlyDictionary{TKey, TValue}.</typeparam>
    /// <returns>An updated Testcontainer configuration.</returns>
    private static IReadOnlyDictionary<T, T> Merge<T>(IReadOnlyDictionary<T, T> myself, IReadOnlyDictionary<T, T> old)
      where T : class
    {
      if (myself == null || old == null)
      {
        return myself ?? old;
      }
      else
      {
        return myself.Concat(old.Where(x => !myself.Keys.Contains(x.Key))).ToDictionary(item => item.Key, item => item.Value);
      }
    }

    /// <summary>
    /// This class represents the Docker container configurations.
    /// </summary>
    public sealed class ContainerConfiguration
    {
      public string Image { get; set; }

      public string Name { get; set; }

      public string WorkingDirectory { get; set; }

      public IReadOnlyCollection<string> Entrypoint { get; set; }

      public IReadOnlyCollection<string> Command { get; set; }

      public IReadOnlyDictionary<string, string> Environments { get; set; }

      public IReadOnlyDictionary<string, string> Labels { get; set; }

      public IReadOnlyDictionary<string, string> ExposedPorts { get; set; }
    }

    /// <summary>
    /// This class represents the Docker host configurations.
    /// </summary>
    public sealed class HostConfiguration
    {
      public IReadOnlyDictionary<string, string> PortBindings { get; set; }

      public IReadOnlyDictionary<string, string> Mounts { get; set; }
    }
  }
}
