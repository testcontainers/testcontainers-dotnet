namespace DotNet.Testcontainers.Core.Builder
{
  using System.Collections.Generic;

  /// <summary>
  /// Holds the host configuration for Testcontainers.
  /// </summary>
  internal class InternalHostConfig
  {
    protected IReadOnlyDictionary<string, string> PortBindings { get; private set; }

    protected IReadOnlyDictionary<string, string> Mounts { get; private set; }

    public void SetPortBindings(IReadOnlyDictionary<string, string> portBindings)
    {
      this.PortBindings = portBindings;
    }

    public void SetMounts(IReadOnlyDictionary<string, string> mounts)
    {
      this.Mounts = mounts;
    }
  }
}
