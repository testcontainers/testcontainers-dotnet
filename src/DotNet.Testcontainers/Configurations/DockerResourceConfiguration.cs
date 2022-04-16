namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;

  /// <inheritdoc cref="IDockerResourceConfiguration" />
  public class DockerResourceConfiguration : IDockerResourceConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerResourceConfiguration" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    public DockerResourceConfiguration(IDockerResourceConfiguration dockerResourceConfiguration)
      : this(dockerResourceConfiguration.Endpoint, dockerResourceConfiguration.Labels)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerResourceConfiguration" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="labels">A list of labels.</param>
    public DockerResourceConfiguration(Uri endpoint = null, IReadOnlyDictionary<string, string> labels = null)
    {
      this.Endpoint = endpoint;
      this.Labels = labels;
    }

    /// <inheritdoc />
    public Uri Endpoint { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }
  }
}
