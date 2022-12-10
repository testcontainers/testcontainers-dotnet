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
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerIgnoreFile" /> class.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    /// <param name="dockerfileFile">Dockerfile file name.</param>
    /// <param name="logger">The logger.</param>
    public DockerIgnoreFile(string dockerIgnoreFileDirectory, string dockerIgnoreFile, string dockerfileFile, ILogger logger)
      : this(new DirectoryInfo(dockerIgnoreFileDirectory), dockerIgnoreFile, dockerfileFile, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerIgnoreFile" /> class.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    /// <param name="dockerfileFile">Dockerfile file name.</param>
    /// <param name="logger">The logger.</param>
    public DockerIgnoreFile(DirectoryInfo dockerIgnoreFileDirectory, string dockerIgnoreFile, string dockerfileFile, ILogger logger)
      : base(GetPatterns(dockerIgnoreFileDirectory, dockerIgnoreFile, dockerfileFile), logger)
    {
    }

    private static IEnumerable<string> GetPatterns(DirectoryInfo dockerIgnoreFileDirectory, string dockerIgnoreFile, string dockerfileFile)
    {
      // These files are necessary and sent to the Docker daemon. The ADD and COPY instructions do not copy them to the image:
      // https://docs.docker.com/engine/reference/builder/#dockerignore-file.
      var negateNecessaryFiles = new[] { dockerIgnoreFile, dockerfileFile }.Select(file => "!" + file);
      return dockerIgnoreFileDirectory.GetFiles(dockerIgnoreFile, SearchOption.TopDirectoryOnly).Any()
        ? File.ReadLines(Path.Combine(dockerIgnoreFileDirectory.FullName, dockerIgnoreFile)).Concat(negateNecessaryFiles)
        : negateNecessaryFiles;
    }
  }
}
