namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Runtime.InteropServices;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class DockerDesktopEndpointAuthenticationProvider : RootlessUnixEndpointAuthenticationProvider, ICustomConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerDesktopEndpointAuthenticationProvider" /> class.
    /// </summary>
    public DockerDesktopEndpointAuthenticationProvider()
      : base(DockerConfig.Instance.GetCurrentEndpoint()?.AbsolutePath, GetSocketPathFromHomeDesktopDir(), GetSocketPathFromHomeRunDir())
    {
    }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && DockerEngine != null;
    }

    /// <inheritdoc />
    public string GetDockerConfig()
    {
      return null;
    }

    /// <inheritdoc />
    public Uri GetDockerHost()
    {
      return null;
    }

    /// <inheritdoc />
    public string GetDockerContext()
    {
      return null;
    }

    /// <inheritdoc />
    public string GetDockerHostOverride()
    {
      return null;
    }

    /// <inheritdoc />
    public string GetDockerSocketOverride()
    {
      // Docker Desktop for Linux and macOS executes containers within a virtual
      // machine. It is important to note that the socket path inside the VM differs
      // from the socket path on the test host. In order to properly mount the
      // appropriate socket to the Resource Reaper, we need to override it specifically
      // for Docker Desktop.
      return "/var/run/docker.sock";
    }

    /// <inheritdoc />
    public JsonDocument GetDockerAuthConfig()
    {
      return null;
    }

    /// <inheritdoc />
    public string GetDockerCertPath()
    {
      return null;
    }

    /// <inheritdoc />
    public bool GetDockerTls()
    {
      return false;
    }

    /// <inheritdoc />
    public bool GetDockerTlsVerify()
    {
      return false;
    }

    /// <inheritdoc />
    public bool GetRyukDisabled()
    {
      return false;
    }

    /// <inheritdoc />
    public bool? GetRyukContainerPrivileged()
    {
      return false;
    }

    /// <inheritdoc />
    public IImage GetRyukContainerImage()
    {
      return null;
    }

    /// <inheritdoc />
    public string GetHubImageNamePrefix()
    {
      return null;
    }

    /// <inheritdoc />
    public ushort? GetWaitStrategyRetries()
    {
      return null;
    }

    /// <inheritdoc />
    public TimeSpan? GetWaitStrategyInterval()
    {
      return null;
    }

    /// <inheritdoc />
    public TimeSpan? GetWaitStrategyTimeout()
    {
      return null;
    }
  }
}
