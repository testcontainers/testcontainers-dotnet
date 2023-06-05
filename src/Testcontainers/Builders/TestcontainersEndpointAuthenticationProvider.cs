namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// When configuring the endpoint for a container runtime, the `DOCKER_HOST`
  /// environment variable is commonly used. This approach can become messy due to
  /// the variety of alternative container runtimes. Even though Testcontainers logs
  /// the container runtime that is being used, developers find it difficult to
  /// determine which runtime is driving the tests on their environment. If multiple
  /// container runtimes are present in a development environment, we prioritize
  /// Testcontainers Cloud if it is running.
  /// </summary>
  [PublicAPI]
  internal sealed class TestcontainersEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider, ICustomConfiguration
  {
    private readonly ICustomConfiguration _customConfiguration;

    private readonly Uri _dockerEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersEndpointAuthenticationProvider" /> class.
    /// </summary>
    public TestcontainersEndpointAuthenticationProvider()
      : this(new TestcontainersConfiguration())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="lines">A list of Java properties file lines.</param>
    public TestcontainersEndpointAuthenticationProvider(params string[] lines)
      : this(new TestcontainersConfiguration(lines))
    {
    }

    private TestcontainersEndpointAuthenticationProvider(ICustomConfiguration customConfiguration)
    {
      _customConfiguration = customConfiguration;
      _dockerEngine = customConfiguration.GetDockerHost();
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

    private sealed class TestcontainersConfiguration : PropertiesFileConfiguration
    {
      public TestcontainersConfiguration()
      {
      }

      public TestcontainersConfiguration(params string[] lines)
        : base(lines)
      {
      }

      protected override Uri GetDockerHost(string propertyName)
      {
        return base.GetDockerHost("tc.host");
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
