namespace DotNet.Testcontainers.Images
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// An implementation of <see cref="IgnoreFile" /> that uses the patterns of the .dockerignore file to ignore directories and files.
  /// </summary>
  internal sealed class DockerIgnoreFile : IgnoreFile
  {
    private const string NegatedDockerFileGlob = "!Dockerfile";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerIgnoreFile" /> class.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    /// <param name="logger">The logger.</param>
    public DockerIgnoreFile(string dockerIgnoreFileDirectory, string dockerIgnoreFile, ILogger logger)
      : this(dockerIgnoreFileDirectory, dockerIgnoreFile, logger, new FileSystem())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerIgnoreFile" /> class.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="fileSystem">The file system abstraction.</param>
    public DockerIgnoreFile(string dockerIgnoreFileDirectory, string dockerIgnoreFile, ILogger logger, IFileSystem fileSystem)
      : base(GetDockerIgnoreFileContents(dockerIgnoreFileDirectory, dockerIgnoreFile, fileSystem), logger)
    {
    }

    /// <summary>
    /// This will read the contents of the .dockerignore file. It will append negated patterns for the Dockerfile and
    /// .dockerignore files so they are always included in the tarball. The docker daemon will make sure not to
    /// include these in the image when they are excluded in the .dockerignore file. If the dockerignore file cannot
    /// be found an empty enumerable will be returned.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <returns>The contents of the .dockerignore file as an array of strings or an empty enumerable when the dockerignore file does not exist.</returns>
    private static IEnumerable<string> GetDockerIgnoreFileContents(string dockerIgnoreFileDirectory, string dockerIgnoreFile, IFileSystem fileSystem)
    {
      if (!fileSystem.FileExists(dockerIgnoreFileDirectory, dockerIgnoreFile))
      {
        return Enumerable.Empty<string>();
      }

      var fullPath = Path.Combine(fileSystem.GetDirectoryFullName(dockerIgnoreFileDirectory), dockerIgnoreFile);
      return fileSystem.ReadLines(fullPath)
        .Concat(new[] { $"!{dockerIgnoreFile}", NegatedDockerFileGlob })
        .ToArray();
    }
  }
}
