namespace DotNet.Testcontainers.Images
{
  using System.IO;
  using System.Linq;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// An implementation of <see cref="IgnoreFile" /> that uses the patterns of the .dockerignore file to ignore directories and files.
  /// </summary>
  internal sealed class DockerIgnoreFile : IgnoreFile
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerIgnoreFile" /> class.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    /// <param name="logger">The logger.</param>
    public DockerIgnoreFile(string dockerIgnoreFileDirectory, string dockerIgnoreFile, ILogger logger)
      : this(new DirectoryInfo(dockerIgnoreFileDirectory), dockerIgnoreFile, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerIgnoreFile" /> class.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    /// <param name="logger">The logger.</param>
    public DockerIgnoreFile(DirectoryInfo dockerIgnoreFileDirectory, string dockerIgnoreFile, ILogger logger)
      : base(
        dockerIgnoreFileDirectory.GetFiles(dockerIgnoreFile, SearchOption.TopDirectoryOnly).Any()
          ? File.ReadLines(Path.Combine(dockerIgnoreFileDirectory.FullName, dockerIgnoreFile)).Concat(new[] { dockerIgnoreFile }).ToArray()
          : new[] { dockerIgnoreFile },
        logger)
    {
    }
  }
}
