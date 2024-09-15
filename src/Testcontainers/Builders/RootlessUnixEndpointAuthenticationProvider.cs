namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Runtime.InteropServices;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IDockerRegistryAuthenticationProvider" />
  [PublicAPI]
  internal class RootlessUnixEndpointAuthenticationProvider : DockerEndpointAuthenticationProvider
  {
    private const string DockerSocket = "docker.sock";

    /// <summary>
    /// Initializes a new instance of the <see cref="RootlessUnixEndpointAuthenticationProvider" /> class.
    /// </summary>
    public RootlessUnixEndpointAuthenticationProvider()
      : this(GetSocketPathFromEnv(), GetSocketPathFromHomeRunDir(), GetSocketPathFromRunDir())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RootlessUnixEndpointAuthenticationProvider" /> class.
    /// </summary>
    /// <param name="socketPaths">A list of socket paths.</param>
    public RootlessUnixEndpointAuthenticationProvider(params string[] socketPaths)
    {
      var socketPath = Array.Find(socketPaths, File.Exists);
      DockerEngine = socketPath == null ? null : new Uri("unix://" + socketPath);
    }

    /// <summary>
    /// Gets the Unix socket Docker Engine endpoint.
    /// </summary>
    protected Uri DockerEngine { get; }

    /// <inheritdoc />
    public override bool IsApplicable()
    {
      return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && DockerEngine != null;
    }

    /// <inheritdoc />
    public override IDockerEndpointAuthenticationConfiguration GetAuthConfig()
    {
      return new DockerEndpointAuthenticationConfiguration(DockerEngine);
    }

    protected static string GetSocketPathFromEnv()
    {
      var xdgRuntimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
      return string.Join("/", xdgRuntimeDir, DockerSocket);
    }

    protected static string GetSocketPathFromHomeDesktopDir()
    {
      return string.Join("/", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker", "desktop", DockerSocket);
    }

    protected static string GetSocketPathFromHomeRunDir()
    {
      return string.Join("/", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker", "run", DockerSocket);
    }

    protected static string GetSocketPathFromRunDir()
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

      return string.Join("/", string.Empty, "user", uid, DockerSocket);
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
