namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// A resource configuration.
  /// </summary>
  /// <typeparam name="TCreateResourceEntity">The underlying Docker.DotNet resource entity.</typeparam>
  [PublicAPI]
  public interface IResourceConfiguration<in TCreateResourceEntity>
  {
    /// <summary>
    /// Gets the test session id.
    /// </summary>
    Guid SessionId { get; }

    /// <summary>
    /// Gets a value indicating whether to reuse an existing resource configuration or not.
    /// </summary>
    bool? Reuse { get; }

    /// <summary>
    /// Gets the Docker endpoint authentication configuration.
    /// </summary>
    IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <summary>
    /// Gets a list of labels.
    /// </summary>
    IReadOnlyDictionary<string, string> Labels { get; }

    /// <summary>
    /// Gets a list of low level modifications of the Docker.DotNet entity.
    /// </summary>
    IReadOnlyList<Action<TCreateResourceEntity>> ParameterModifiers { get; }

    /// <summary>
    /// Gets the reuse hash.
    /// </summary>
    string GetReuseHash();
  }
}
