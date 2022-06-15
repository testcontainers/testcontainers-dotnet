namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// An abstract fluent Docker resource builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  [PublicAPI]
  public interface IAbstractBuilder<out TBuilderEntity>
  {
    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <see cref="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(string endpoint);

    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <see cref="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(Uri endpoint);

    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    /// <returns>A configured instance of <see cref="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig);

    /// <summary>
    /// If true, the <see cref="ResourceReaper" /> will remove the Docker resource automatically. Otherwise, the Docker resource will be kept.
    /// </summary>
    /// <param name="cleanUp">True, the <see cref="ResourceReaper" /> will remove the Docker resource automatically. Otherwise, the Docker resource will be kept.</param>
    /// <returns>A configured instance of <see cref="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithCleanUp(bool cleanUp);

    /// <summary>
    /// Adds user-defined metadata to the Docker resource.
    /// </summary>
    /// <param name="name">Label name.</param>
    /// <param name="value">Label value.</param>
    /// <returns>A configured instance of <see cref="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithLabel(string name, string value);

    /// <summary>
    /// Sets the Resource Reaper session id.
    /// </summary>
    /// <param name="resourceReaperSessionId">The session id of the <see cref="ResourceReaper" /> instance.</param>
    /// <returns>A configured instance of <see cref="TBuilderEntity" />.</returns>
    /// <remarks>The <see cref="ResourceReaper" /> will delete the Docker resource after the tests has been finished.</remarks>
    [PublicAPI]
    TBuilderEntity WithResourceReaperSessionId(Guid resourceReaperSessionId);
  }
}
