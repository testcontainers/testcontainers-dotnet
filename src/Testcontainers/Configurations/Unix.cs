namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// Unix operating system.
  /// </summary>
  [PublicAPI]
  public sealed class Unix : IOperatingSystem
  {
    /// <summary>
    /// Represents the Unix file mode 644, which grants read and write permissions to the user and read permissions to the group and others.
    /// </summary>
    public const UnixFileModes FileMode644 =
      UnixFileModes.UserRead |
      UnixFileModes.UserWrite |
      UnixFileModes.GroupRead |
      UnixFileModes.OtherRead;

    /// <summary>
    /// Represents the Unix file mode 666, which grants read and write permissions to the user, group, and others.
    /// </summary>
    public const UnixFileModes FileMode666 =
      UnixFileModes.UserRead |
      UnixFileModes.UserWrite |
      UnixFileModes.GroupRead |
      UnixFileModes.GroupWrite |
      UnixFileModes.OtherRead |
      UnixFileModes.OtherWrite;

    /// <summary>
    /// Represents the Unix file mode 700, which grants read, write, and execute permissions to the user, and no permissions to the group and others.
    /// </summary>
    public const UnixFileModes FileMode700 =
      UnixFileModes.UserRead |
      UnixFileModes.UserWrite |
      UnixFileModes.UserExecute;

    /// <summary>
    /// Represents the Unix file mode 755, which grants read, write, and execute permissions to the user, and read and execute permissions to the group and others.
    /// </summary>
    public const UnixFileModes FileMode755 =
      UnixFileModes.UserRead |
      UnixFileModes.UserWrite |
      UnixFileModes.UserExecute |
      UnixFileModes.GroupRead |
      UnixFileModes.GroupExecute |
      UnixFileModes.OtherRead |
      UnixFileModes.OtherExecute;

    /// <summary>
    /// Represents the Unix file mode 777, which grants read, write, and execute permissions to the user, group, and others.
    /// </summary>
    public const UnixFileModes FileMode777 =
      UnixFileModes.UserRead |
      UnixFileModes.UserWrite |
      UnixFileModes.UserExecute |
      UnixFileModes.GroupRead |
      UnixFileModes.GroupWrite |
      UnixFileModes.GroupExecute |
      UnixFileModes.OtherRead |
      UnixFileModes.OtherWrite |
      UnixFileModes.OtherExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    [PublicAPI]
    public Unix()
      : this(UnixEndpointAuthenticationProvider.DockerEngine)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Unix(string endpoint)
      : this(new Uri(endpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    [PublicAPI]
    public Unix(Uri endpoint)
      : this(new DockerEndpointAuthenticationConfiguration(endpoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Unix" /> class.
    /// </summary>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    [PublicAPI]
    public Unix(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      DockerEndpointAuthConfig = dockerEndpointAuthConfig;
    }

    /// <summary>
    /// Gets the <see cref="IOperatingSystem" /> instance.
    /// </summary>
    public static IOperatingSystem Instance { get; }
      = new Unix(dockerEndpointAuthConfig: null);

    /// <inheritdoc />
    public IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path?.Replace('\\', '/');
    }
  }
}
