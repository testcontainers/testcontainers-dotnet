namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Configurations;
  using ICSharpCode.SharpZipLib.Tar;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// Generates a tar archive with docker configuration files. The generated tar archive can be used to build a docker image.
  /// </summary>
  internal sealed class DockerfileArchive : ITarArchive
  {
    private static readonly IOperatingSystem OS = new Unix();

    private readonly DirectoryInfo dockerfileDirectory;

    private readonly IDockerImage image;

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerfileArchive" /> class.
    /// </summary>
    /// <param name="dockerfileDirectory">Directory to docker configuration files.</param>
    /// <param name="dockerfile">Name of the dockerfile, which is necessary to start the docker build.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    /// <param name="logger">The logger.</param>
    public DockerfileArchive(string dockerfileDirectory, string dockerfile, IDockerImage image, ILogger logger)
      : this(new DirectoryInfo(dockerfileDirectory), dockerfile, image, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerfileArchive" /> class.
    /// </summary>
    /// <param name="dockerfileDirectory">Directory to docker configuration files.</param>
    /// <param name="dockerfile">Name of the dockerfile, which is necessary to start the docker build.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Will be thrown if the dockerfile directory does not exist or the directory does not contain a dockerfile.</exception>
    public DockerfileArchive(DirectoryInfo dockerfileDirectory, string dockerfile, IDockerImage image, ILogger logger)
    {
      if (!dockerfileDirectory.Exists)
      {
        throw new ArgumentException($"Directory '{dockerfileDirectory.FullName}' does not exist.");
      }

      if (!dockerfileDirectory.GetFiles(dockerfile, SearchOption.TopDirectoryOnly).Any())
      {
        throw new ArgumentException($"{dockerfile} does not exist in '{dockerfileDirectory.FullName}'.");
      }

      this.dockerfileDirectory = dockerfileDirectory;
      this.image = image;
      this.logger = logger;
    }

    /// <inheritdoc />
    public string Tar()
    {
      var dockerfileArchiveName = Regex.Replace(this.image.FullName, "[^a-zA-Z0-9]", "-").ToLowerInvariant();

      var dockerfileArchivePath = Path.Combine(Path.GetTempPath(), $"{dockerfileArchiveName}.tar");

      var dockerIgnoreFile = new DockerIgnoreFile(this.dockerfileDirectory.FullName, ".dockerignore", this.logger);

      using (var stream = new FileStream(dockerfileArchivePath, FileMode.Create))
      {
        using (var tarArchive = TarArchive.CreateOutputTarArchive(stream))
        {
          tarArchive.RootPath = OS.NormalizePath(this.dockerfileDirectory.FullName);

          foreach (var file in GetFiles(this.dockerfileDirectory.FullName))
          {
            var relativePath = file.Substring(tarArchive.RootPath.Length + 1);

            if (dockerIgnoreFile.Denies(relativePath))
            {
              continue;
            }

            var tarEntry = TarEntry.CreateEntryFromFile(file);
            tarEntry.Name = relativePath;
            tarArchive.WriteEntry(tarEntry, true);
          }
        }
      }

      return dockerfileArchivePath;
    }

    /// <summary>
    /// Gets all accepted docker archive files.
    /// </summary>
    /// <param name="directory">Directory to docker configuration files.</param>
    /// <returns>Returns a list with all accepted docker archive files.</returns>
    private static IEnumerable<string> GetFiles(string directory)
    {
      return Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
        .AsParallel()
        .Select(Path.GetFullPath)
        .Select(OS.NormalizePath)
        .ToArray();
    }
  }
}
