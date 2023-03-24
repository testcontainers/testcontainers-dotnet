namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Provides data for the <see cref="ResourceReaper.StateChanged" /> event.
  /// </summary>
  [PublicAPI]
  public sealed class ResourceReaperStateEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceReaperStateEventArgs" /> class.
    /// </summary>
    /// <param name="resourceReaper">The Resource Reaper instance.</param>
    /// <param name="resourceReaperState">The Resource Reaper state.</param>
    public ResourceReaperStateEventArgs(ResourceReaper resourceReaper, ResourceReaperState resourceReaperState)
    {
      Instance = resourceReaper;
      State = resourceReaperState;
    }

    /// <summary>
    /// Gets the <see cref="ResourceReaper" /> instance.
    /// </summary>
    [PublicAPI]
    public ResourceReaper Instance { get; }

    /// <summary>
    /// Gets the <see cref="ResourceReaper" /> state.
    /// </summary>
    [PublicAPI]
    public ResourceReaperState State { get; }
  }
}
