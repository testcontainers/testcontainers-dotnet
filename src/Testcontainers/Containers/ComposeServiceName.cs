namespace DotNet.Testcontainers.Containers
{
  using System.Collections.Generic;
  using System.Globalization;
  using DotNet.Testcontainers.Builders;

  /// <summary>
  /// Resolves the Docker Compose service instance of a Docker Compose container.
  /// </summary>
  /// <remarks>
  /// A Docker Compose service can run more than one container (see
  /// <see cref="ComposeBuilder.WithScaledService" />). Each container is one
  /// instance of the service, addressed by the service name and the container
  /// number. The service name always addresses the service as it is declared in
  /// the Docker Compose file. The instance number addresses one of its
  /// containers.
  /// </remarks>
  internal static class ComposeServiceName
  {
    /// <summary>
    /// The label that contains the Docker Compose service name.
    /// </summary>
    public const string ServiceLabel = "com.docker.compose.service";

    /// <summary>
    /// The label that contains the number of the container within the Docker
    /// Compose service.
    /// </summary>
    public const string ContainerNumberLabel = "com.docker.compose.container-number";

    /// <summary>
    /// The instance that a Docker Compose service name addresses if no instance is
    /// set.
    /// </summary>
    public const ushort FirstInstance = 1;

    /// <summary>
    /// Gets the Docker Compose service instance of a Docker Compose container.
    /// </summary>
    /// <param name="labels">The labels of the Docker Compose container.</param>
    /// <returns>The Docker Compose service instance, or <c>null</c> if the container does not belong to a Docker Compose service.</returns>
    public static (string ServiceName, ushort Instance)? GetInstance(IDictionary<string, string> labels)
    {
      if (labels == null || !labels.TryGetValue(ServiceLabel, out var serviceName))
      {
        return null;
      }

      // Docker Compose sets the container number for every service container.
      // Fall back to the first instance if a container does not carry the label.
      if (!labels.TryGetValue(ContainerNumberLabel, out var containerNumber) || !ushort.TryParse(containerNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out var instance))
      {
        instance = FirstInstance;
      }

      return (serviceName, instance);
    }

    /// <summary>
    /// Gets the display name of a Docker Compose service instance, e.g. <c>web-2</c>.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <returns>The display name of the Docker Compose service instance.</returns>
    public static string GetDisplayName(string serviceName, ushort instance)
    {
      return serviceName + "-" + instance.ToString(CultureInfo.InvariantCulture);
    }
  }
}
