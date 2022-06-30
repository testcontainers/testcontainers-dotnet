namespace DotNet.Testcontainers.Containers
{
  public enum ResourceReaperState
  {
    /// <summary>
    /// <see cref="ResourceReaper" /> is created.
    /// </summary>
    Created,

    /// <summary>
    /// <see cref="ResourceReaper" /> initializes the TCP connection to Ryuk.
    /// </summary>
    InitializingConnection,

    /// <summary>
    /// <see cref="ResourceReaper" /> maintains the TCP connection to Ryuk.
    /// </summary>
    /// <remarks>
    /// <see cref="ResourceReaper.GetAndStartNewAsync(DotNet.Testcontainers.Configurations.IDockerEndpointAuthenticationConfiguration, string, System.TimeSpan, System.Threading.CancellationToken)" /> will complete now.
    /// </remarks>
    MaintainingConnection,

    /// <summary>
    /// The connection to Ryuk has been terminated and Ryuk is going to clean up all associated Docker resources.
    /// </summary>
    ConnectionTerminated,
  }
}
