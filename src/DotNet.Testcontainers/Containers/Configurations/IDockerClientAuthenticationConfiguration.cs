namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// An authentication configuration to authenticate against Docker hosts (TLS).
  /// </summary>
  public interface IDockerClientAuthenticationConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// True if the auth configuration is applicable, otherwise false.
    /// </summary>
    bool IsApplicable { get; }

    /// <summary>
    /// True if TLS verification is enabled, otherwise false.
    /// </summary>
    bool IsTlsVerificationEnabled { get; }

    /// <summary>
    /// Gets the TLS certificates base directory.
    /// </summary>
    [CanBeNull]
    string CertificatesDirectory { get; }
  }
}
