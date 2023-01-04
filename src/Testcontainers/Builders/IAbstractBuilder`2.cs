﻿namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker resource builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  /// <typeparam name="TResourceEntity">The resource entity.</typeparam>
  [PublicAPI]
  public interface IAbstractBuilder<out TBuilderEntity, out TResourceEntity>
  {
    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(string endpoint);

    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(Uri endpoint);

    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig);

    /// <summary>
    /// Cleans up the Docker resource after the tests are finished.
    /// </summary>
    /// <param name="cleanUp">Determines whether the <see cref="ResourceReaper" /> removes the Docker resource after the tests are finished or not.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithCleanUp(bool cleanUp);

    /// <summary>
    /// Adds user-defined metadata to the Docker resource.
    /// </summary>
    /// <param name="name">The label name.</param>
    /// <param name="value">The label value.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithLabel(string name, string value);

    /// <summary>
    /// Adds user-defined metadata to the Docker resource.
    /// </summary>
    /// <param name="labels">A dictionary of environment variables.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithLabel(IReadOnlyDictionary<string, string> labels);

    /// <summary>
    /// Sets the Resource Reaper session id.
    /// </summary>
    /// <param name="resourceReaperSessionId">The <see cref="ResourceReaper" /> session id.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithResourceReaperSessionId(Guid resourceReaperSessionId);

    /// <summary>
    /// Builds an instance of <typeparamref name="TResourceEntity" /> with the given resource configuration.
    /// </summary>
    /// <returns>A configured instance of <typeparamref name="TResourceEntity" />.</returns>
    [PublicAPI]
    TResourceEntity Build();
  }
}