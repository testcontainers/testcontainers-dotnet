namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class TestcontainersHostEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider, ICustomConfiguration
  {
    private readonly ICustomConfiguration _customConfiguration = new TestcontainersHostConfiguration();

    private readonly Uri _dockerEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersHostEndpointAuthenticationProvider" /> class.
    /// </summary>
    public TestcontainersHostEndpointAuthenticationProvider()
    {
      _dockerEngine = GetDockerHost();
    }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return _dockerEngine != null && "tcp".Equals(_dockerEngine.Scheme, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(_dockerEngine);
    }

    /// <inheritdoc />
    public string GetDockerConfig()
    {
      return _customConfiguration.GetDockerConfig();
    }

    /// <inheritdoc />
    public Uri GetDockerHost()
    {
      return _customConfiguration.GetDockerHost();
    }

    /// <inheritdoc />
    public string GetDockerHostOverride()
    {
      return _customConfiguration.GetDockerHostOverride();
    }

    /// <inheritdoc />
    public string GetDockerSocketOverride()
    {
      return _customConfiguration.GetDockerSocketOverride();
    }

    /// <inheritdoc />
    public JsonDocument GetDockerAuthConfig()
    {
      return _customConfiguration.GetDockerAuthConfig();
    }

    /// <inheritdoc />
    public string GetDockerCertPath()
    {
      return _customConfiguration.GetDockerCertPath();
    }

    /// <inheritdoc />
    public bool GetDockerTls()
    {
      return _customConfiguration.GetDockerTls();
    }

    /// <inheritdoc />
    public bool GetDockerTlsVerify()
    {
      return _customConfiguration.GetDockerTlsVerify();
    }

    /// <inheritdoc />
    public bool GetRyukDisabled()
    {
      return _customConfiguration.GetRyukDisabled();
    }

    /// <inheritdoc />
    public bool GetRyukContainerPrivileged()
    {
      return _customConfiguration.GetRyukContainerPrivileged();
    }

    /// <inheritdoc />
    public IImage GetRyukContainerImage()
    {
      return _customConfiguration.GetRyukContainerImage();
    }

    /// <inheritdoc />
    public string GetHubImageNamePrefix()
    {
      return _customConfiguration.GetHubImageNamePrefix();
    }

    private sealed class TestcontainersHostConfiguration : PropertiesFileConfiguration
    {
      protected override Uri GetDockerHost(string propertyName)
      {
        return base.GetDockerHost("testcontainers.host");
      }

      protected override string GetDockerHostOverride(string propertyName)
      {
        return null;
      }

      protected override string GetDockerSocketOverride(string propertyName)
      {
        return null;
      }
    }
  }
}
