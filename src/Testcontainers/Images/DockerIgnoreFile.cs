namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Collections.Generic;
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
    /// <param name="dockerignoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerignoreFile">.dockerignore file name.</param>
    /// <param name="dockerfileFile">Dockerfile file name.</param>
    /// <param name="logger">The logger.</param>
    public DockerIgnoreFile(string dockerignoreFileDirectory, string dockerignoreFile, string dockerfileFile, ILogger logger)
      : this(new DirectoryInfo(dockerignoreFileDirectory), dockerignoreFile, dockerfileFile, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerIgnoreFile" /> class.
    /// </summary>
    /// <param name="dockerignoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerignoreFile">.dockerignore file name.</param>
    /// <param name="dockerfileFile">Dockerfile file name.</param>
    /// <param name="logger">The logger.</param>
    public DockerIgnoreFile(FileSystemInfo dockerignoreFileDirectory, string dockerignoreFile, string dockerfileFile, ILogger logger)
      : base(GetPatterns(dockerignoreFileDirectory, dockerignoreFile, dockerfileFile), logger)
    {
    }

    private static IEnumerable<string> GetPatterns(FileSystemInfo dockerignoreFileDirectory, string dockerignoreFile, string dockerfileFile)
    {
      var dockerignoreFilePath = Path.Combine(dockerignoreFileDirectory.FullName, dockerignoreFile);

      // These files are necessary and sent to the Docker daemon. The ADD and COPY instructions do not copy them to the image:
      // https://docs.docker.com/engine/reference/builder/#dockerignore-file.
      var negateNecessaryFiles = new[] { dockerignoreFile, dockerfileFile }
        .Select(file => "!" + file);

      var dockerignorePatterns = File.Exists(dockerignoreFilePath)
        ? File.ReadLines(dockerignoreFilePath)
        : Array.Empty<string>();

      return new[] { "**/.idea", "**/.vs" }
        .Concat(dockerignorePatterns)
        .Concat(negateNecessaryFiles);
    }
  }
}
