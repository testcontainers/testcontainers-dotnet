namespace DotNet.Testcontainers.Builders
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
  /// <typeparam name="TCreateResourceEntity">The underlying Docker.DotNet resource entity.</typeparam>
  [PublicAPI]
  public interface IAbstractBuilder<out TBuilderEntity, out TResourceEntity, out TCreateResourceEntity>
  {
    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <remarks>
    /// Testcontainers automatically discovers the Docker environment and applies the configuration.
    /// It is recommended to use the auto discovery. Only certain edge cases require this enhanced API.
    /// </remarks>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(string endpoint);

    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <remarks>
    /// Testcontainers automatically discovers the Docker environment and applies the configuration.
    /// It is recommended to use the auto discovery. Only certain edge cases require this enhanced API.
    /// </remarks>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithDockerEndpoint(Uri endpoint);

    /// <summary>
    /// Sets the Docker API endpoint.
    /// </summary>
    /// <remarks>
    /// Testcontainers automatically discovers the Docker environment and applies the configuration.
    /// It is recommended to use the auto discovery. Only certain edge cases require this enhanced API.
    /// </remarks>
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
    /// Reuses an existing Docker resource.
    /// </summary>
    /// <remarks>
    /// If reuse is enabled, Testcontainers will label the resource with a hash value
    /// according to the respective build/resource configuration. When Testcontainers finds a
    /// matching resource, it will reuse this resource instead of creating a new one. Enabling
    /// reuse will disable the resource reaper, meaning the resource will not be cleaned up
    /// after the tests are finished.
    ///
    /// This is an <b>experimental</b> feature. Reuse does not take all builder
    /// configurations into account when calculating the hash value. There might be configurations
    /// where Testcontainers is not, or not yet, able to find a matching resource and
    /// recreate the resource.
    /// </remarks>
    /// <param name="reuse">Determines whether to reuse an existing resource configuration or not.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithReuse(bool reuse);

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
    /// Allows low level modifications of the Docker.DotNet entity after the builder configuration has been applied. Multiple low level modifications will be executed in order of insertion.
    /// </summary>
    /// <remarks>
    /// This API is intended for special use cases only. We do not recommend using it and cannot maintain backwards compatibility.
    /// </remarks>
    /// <param name="parameterModifier">The action that invokes modifying the Docker.DotNet entity instance.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithCreateParameterModifier(Action<TCreateResourceEntity> parameterModifier);

    /// <summary>
    /// Builds an instance of <typeparamref name="TResourceEntity" /> with the given resource configuration.
    /// </summary>
    /// <returns>A configured instance of <typeparamref name="TResourceEntity" />.</returns>
    /// <exception cref="ArgumentException">Thrown when a mandatory Docker resource configuration is not set.</exception>
    [PublicAPI]
    TResourceEntity Build();
  }
}
