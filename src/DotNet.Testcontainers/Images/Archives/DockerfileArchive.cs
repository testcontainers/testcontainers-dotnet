namespace DotNet.Testcontainers.Images.Archives
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Services;
  using ICSharpCode.SharpZipLib.Tar;

  /// <summary>
  /// Generates a tar archive with docker configuration files. The generated tar archive can be used to build a docker image.
  /// </summary>
  internal sealed class DockerfileArchive : ITarArchive
  {
    private static readonly IOperatingSystem os = new Unix();

    private readonly DirectoryInfo dockerfileDirectory;

    private readonly IDockerImage image;

    /// <summary>
    /// Creates an instance of <see cref="DockerfileArchive" /> for the specified parameters.
    /// </summary>
    /// <param name="dockerfileDirectory">Directory to docker configuration files.</param>
    /// <param name="dockerfile">Name of the dockerfile, which is necessary to start the docker build.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    public DockerfileArchive(string dockerfileDirectory, string dockerfile, IDockerImage image)
      : this(new DirectoryInfo(dockerfileDirectory), dockerfile, image)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="DockerfileArchive" /> for the specified parameters.
    /// </summary>
    /// <param name="dockerfileDirectory">Directory to docker configuration files.</param>
    /// <param name="dockerfile">Name of the dockerfile, which is necessary to start the docker build.</param>
    /// <param name="image">Docker image information to create the tar archive for.</param>
    /// <exception cref="ArgumentException">Will be thrown if the dockerfile directory does not exist or the directory does not contain a dockerfile.</exception>
    public DockerfileArchive(DirectoryInfo dockerfileDirectory, string dockerfile, IDockerImage image)
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
    }

    /// <inheritdoc />
    public string Tar()
    {
      var dockerfileArchiveName = Regex.Replace(this.image.FullName, "[^a-zA-Z0-9]", "-").ToLowerInvariant();

      var dockerfileArchivePath = Path.Combine(Path.GetTempPath(), $"{dockerfileArchiveName}.tar");

      var dockerIgnoreFile = new DockerIgnoreFile(this.dockerfileDirectory.FullName, ".dockerignore");

      using (var stream = new FileStream(dockerfileArchivePath, FileMode.Create))
      {
        using (var tarArchive = TarArchive.CreateOutputTarArchive(stream))
        {
          tarArchive.RootPath = os.NormalizePath(this.dockerfileDirectory.FullName);

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
        .Select(os.NormalizePath)
        .ToArray();
    }
  }
}
