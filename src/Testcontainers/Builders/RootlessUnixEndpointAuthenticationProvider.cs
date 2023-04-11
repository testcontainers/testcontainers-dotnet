namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal sealed class RootlessUnixEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    private readonly Uri _dockerEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootlessUnixEndpointAuthenticationProvider" /> class.
    /// </summary>
    public RootlessUnixEndpointAuthenticationProvider()
      : this(GetSocketPathFromEnv(), GetSocketPathFromHomeDesktopDir(), GetSocketPathFromHomeRunDir(), GetSocketPathFromRunDir())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RootlessUnixEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="socketPaths">A list of socket paths.</param>
    public RootlessUnixEndpointAuthenticationProvider(params string[] socketPaths)
    {
      _dockerEngine = socketPaths
        .Where(File.Exists)
        .Select(socketPath => new UriBuilder("unix", socketPath))
        .Select(uriBuilder => uriBuilder.Uri)
        .FirstOrDefault();
    }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return _dockerEngine != null;
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(_dockerEngine);
    }

    private static string GetSocketPathFromEnv()
    {
      var xdgRuntimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
      return string.Join("/", xdgRuntimeDir, "docker.sock");
    }

    private static string GetSocketPathFromHomeDesktopDir()
    {
      return string.Join("/", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker", "desktop", "docker.sock");
    }

    private static string GetSocketPathFromHomeRunDir()
    {
      return string.Join("/", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker", "run", "docker.sock");
    }

    private static string GetSocketPathFromRunDir()
    {
      ushort uid = 0;

      if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      {
        uid = new Darwin().GetUid();
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        uid = new Linux().GetUid();
      }

      return string.Join("/", string.Empty, "user", uid, "docker.sock");
    }

    /// <summary>
    /// A user identity.
    /// </summary>
    [PublicAPI]
    private interface IUserIdentity
    {
      /// <summary>
      /// Gets the real user ID of the calling process.
      /// </summary>
      /// <returns>The real user ID of the calling process.</returns>
      ushort GetUid();
    }

    /// <inheritdoc cref="IUserIdentity" />
    [PublicAPI]
    private sealed class Darwin : IUserIdentity
    {
      [DllImport("libSystem")]
      private static extern ushort getuid();

      /// <inheritdoc />
      public ushort GetUid()
      {
        return getuid();
      }
    }

    /// <inheritdoc cref="IUserIdentity" />
    [PublicAPI]
    private sealed class Linux : IUserIdentity
    {
      [DllImport("libc")]
      private static extern ushort getuid();

      /// <inheritdoc />
      public ushort GetUid()
      {
        return getuid();
      }
    }
  }
}
