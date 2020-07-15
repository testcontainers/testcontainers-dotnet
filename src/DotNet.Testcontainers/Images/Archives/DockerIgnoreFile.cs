namespace DotNet.Testcontainers.Images.Archives
{
  using System.IO;
  using System.Linq;

  /// <summary>
  /// A implementation of <see cref="IgnoreFile" /> that uses the patterns of the .dockerignore file.
  /// </summary>
  internal sealed class DockerIgnoreFile : IgnoreFile
  {
    /// <summary>
    /// Creates an instance of <see cref="DockerIgnoreFile" /> from the specified directory and ignore files.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    public DockerIgnoreFile(string dockerIgnoreFileDirectory, string dockerIgnoreFile)
      : this(new DirectoryInfo(dockerIgnoreFileDirectory), dockerIgnoreFile)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="DockerIgnoreFile" /> from the specified directory and ignore files.
    /// </summary>
    /// <param name="dockerIgnoreFileDirectory">Directory that contains all docker configuration files.</param>
    /// <param name="dockerIgnoreFile">.dockerignore file name.</param>
    public DockerIgnoreFile(DirectoryInfo dockerIgnoreFileDirectory, string dockerIgnoreFile)
      : base(dockerIgnoreFileDirectory.GetFiles(dockerIgnoreFile, SearchOption.TopDirectoryOnly).Any()
        ? File.ReadLines(Path.Combine(dockerIgnoreFileDirectory.FullName, dockerIgnoreFile)).Concat(new[] { dockerIgnoreFile }).ToArray()
        : new[] { dockerIgnoreFile })
    {
    }
  }
}
