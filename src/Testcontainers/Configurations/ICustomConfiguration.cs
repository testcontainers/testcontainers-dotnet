namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Text.Json;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A Testcontainers custom configuration.
  /// </summary>
  internal interface ICustomConfiguration
  {
    /// <summary>
    /// Gets the Docker config custom configuration.
    /// </summary>
    /// <returns>The Docker config custom configuration.</returns>
    /// <remarks>https://www.testcontainers.org/features/configuration/#customizing-docker-host-detection.</remarks>
    [CanBeNull]
    string GetDockerConfig();

    /// <summary>
    /// Gets the Docker host custom configuration.
    /// </summary>
    /// <returns>The Docker host custom configuration.</returns>
    /// <remarks>https://www.testcontainers.org/features/configuration/#customizing-docker-host-detection.</remarks>
    [CanBeNull]
    Uri GetDockerHost();

    /// <summary>
    /// Gets the Docker registry authentication custom configuration.
    /// </summary>
    /// <returns>The Docker authentication custom configuration.</returns>
    /// <remarks>https://docs.gitlab.com/ee/ci/docker/using_docker_images.html#access-an-image-from-a-private-container-registry.</remarks>
    [CanBeNull]
    JsonDocument GetDockerAuthConfig();

    /// <summary>
    /// Gets the Ryuk disabled custom configuration.
    /// </summary>
    /// <returns>The Ryuk disabled custom configuration.</returns>
    /// <remarks>https://www.testcontainers.org/features/configuration/#customizing-ryuk-resource-reaper.</remarks>
    bool GetRyukDisabled();

    /// <summary>
    /// Gets the Ryuk container image custom configuration.
    /// </summary>
    /// <returns>The Ryuk container image custom configuration.</returns>
    /// <remarks>https://www.testcontainers.org/features/configuration/#customizing-ryuk-resource-reaper.</remarks>
    [CanBeNull]
    IDockerImage GetRyukContainerImage();

    /// <summary>
    /// Gets the Docker Hub image name prefix custom configuration.
    /// </summary>
    /// <returns>The Docker Hub image name prefix custom configuration.</returns>
    /// <remarks>https://www.testcontainers.org/features/image_name_substitution/.</remarks>
    [CanBeNull]
    string GetHubImageNamePrefix();
  }
}
