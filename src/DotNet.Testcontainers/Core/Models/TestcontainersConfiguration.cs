namespace DotNet.Testcontainers.Core.Models
{
  using System.Collections.Generic;
  using System.Linq;
  using DotNet.Testcontainers.Core.Wait;
  using DotNet.Testcontainers.Diagnostics;

  public class TestcontainersConfiguration
  {
    private static readonly IWaitUntil DefaultWaitStrategy = Wait.UntilContainerIsRunning();

    public ContainerConfiguration Container { get; set; } = new ContainerConfiguration();

    public HostConfiguration Host { get; set; } = new HostConfiguration();

    public bool CleanUp { get; set; } = true;

    public IOutputConsumer OutputConsumer { get; set; }

    public IWaitUntil WaitStrategy { get; set; } = DefaultWaitStrategy;

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

      this.OutputConsumer = Merge(this.OutputConsumer, old.OutputConsumer);

      this.WaitStrategy = Merge(this.WaitStrategy, old.WaitStrategy, DefaultWaitStrategy);

      return this;
    }

    private static T Merge<T>(T myself, T old, T defaultConfiguration = null)
      where T : class
    {
      return (myself == null || myself.Equals(defaultConfiguration)) ? old : myself;
    }

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

    public class ContainerConfiguration
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

    public class HostConfiguration
    {
      public IReadOnlyDictionary<string, string> PortBindings { get; set; }

      public IReadOnlyDictionary<string, string> Mounts { get; set; }
    }
  }
}
